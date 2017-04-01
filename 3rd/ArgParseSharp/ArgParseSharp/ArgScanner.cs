using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace ArgParseSharp
{
	public enum TokenType : byte {
		ShortOption,
		LongOption,
		BareArg
	}

	public struct Token {
		public TokenType Type;
		public object Data;

		public Token(TokenType type, object data) {
			this.Type = type;
			this.Data = data;
		}

		public override string ToString() {
			return string.Format("Token(type: {0}, data: {1})", this.Type.ToString(), this.Data);
		}
	}

	internal static class ArgScanner {
		enum ScannerState : byte {
			Unstarted,
			InOptionToken,
			InShortArg,
			InLongArg,
			InBareArg,
			InQuoteString,
			InEscapeChar,
			NeedsNextBare,
			NeedsNextQuote
		}

		public const char tokenOptionChar = '-';
		public const char assignmentOpChar = '=';
		public const char doubleQuoteChar = '"';
		public const char singleQuoteChar = '\'';
		public const char escapeNextChar = '\\';

		static StringBuilder tokenBuilder = new StringBuilder();
		static ScannerState state;
		static ScannerState escapeReturnState;

		private static Token NextTokenFromArg(string s, ref int idx) {
			switch (state) {
				case ScannerState.NeedsNextBare:
					state = ScannerState.InBareArg;
					break;
				case ScannerState.NeedsNextQuote:
					tokenBuilder.Append(' ');
					state = ScannerState.InQuoteString;
					break;
				case ScannerState.InShortArg:
					tokenBuilder.Length = 0;
					break;
				default:
					state = ScannerState.Unstarted;
					tokenBuilder.Length = 0;
					break;
			}
			Token token;

			while (idx < s.Length) {
				char c = s[idx];
				switch (state) {
					case ScannerState.Unstarted:
						switch (c) {
							case tokenOptionChar:
								state = ScannerState.InOptionToken;
								idx++;
								break;
							case doubleQuoteChar:
							case singleQuoteChar:
								idx++;
								state = ScannerState.InQuoteString;
								break;
							default:
								state = ScannerState.InBareArg;
								break;
						}
						break;
					case ScannerState.InOptionToken:
						switch (c) {
							case tokenOptionChar:
								state = ScannerState.InLongArg;
								idx++;
								break;
							default:
								if (Char.IsWhiteSpace(c)) {
									// If we get a single '-' followed by whitespace, figure it's a bare arg
									// (like a stdin placeholder).
									idx++;
									return new Token(TokenType.BareArg, c);
								}
								else {
									state = ScannerState.InShortArg;
								}
								break;
						}
						break;
					case ScannerState.InShortArg:
						if (Char.IsLetterOrDigit(c)) {
							idx++;
							return new Token(TokenType.ShortOption, c);
						}
						else {
							throw new Exception(string.Format(
								"Unexpected character '{0}' at position {1}.  Short options must be letters or digits.",
								c,
								idx
							));
						}
					case ScannerState.InLongArg:
						if (Char.IsWhiteSpace(c) || c == assignmentOpChar) {
							idx++;
							// The token is done, return it.
							token = new Token(TokenType.LongOption, tokenBuilder.ToString());
							tokenBuilder.Length = 0;
							return token;
						}
						else {
							// Keep adding string data to the token.
							tokenBuilder.Append(c);
							idx++;
						}
						break;
					case ScannerState.InQuoteString:
						switch (c) {
							case doubleQuoteChar:
							case singleQuoteChar:
								idx++;
								return new Token(TokenType.BareArg, tokenBuilder.ToString());
							case escapeNextChar:
								state = ScannerState.InEscapeChar;
								escapeReturnState = ScannerState.InQuoteString;
								break;
							default:
								idx++;
								tokenBuilder.Append(c);
								break;
						}
						break;
					case ScannerState.InBareArg:
						switch (c) {
							case escapeNextChar:
								state = ScannerState.InEscapeChar;
								escapeReturnState = ScannerState.InBareArg;
								break;
							default:
								// Keep adding string data to the token until the end of the arg.
								tokenBuilder.Append(c);
								idx++;
								break;
						}
						break;
					case ScannerState.InEscapeChar:
						string escapeSeq = s.SafeSubstring(idx, 5);
						if (idx == (s.Length - 1)) {
							escapeSeq = string.Concat(escapeSeq, ' ');
							state = ScannerState.NeedsNextBare;
						}
						else {
							state = escapeReturnState;
						}
						try {
							int unescapedLen = escapeSeq.Length;
							escapeSeq = Regex.Unescape(escapeSeq);
							int escapedLen = unescapedLen - escapeSeq.Length;
							idx += escapedLen;
							tokenBuilder.Append(escapeSeq[0]);
						}
						catch (ArgumentException) {
							tokenBuilder.Append(c);
						}

						idx++;

						break;
				}
			}

			// If we get here, we've hit the end of the string.
			// If the token builder has zero length, something is malformed.
			if (tokenBuilder.Length < 1) {
				throw new Exception(string.Format("Unexpected end of arg while parsing {0}.", state.ToString()));
			}

			switch (state) {
				case ScannerState.InQuoteString:
					state = ScannerState.NeedsNextQuote;
					return new Token();
				case ScannerState.NeedsNextBare:
					return new Token();
				case ScannerState.InBareArg:
					return new Token(TokenType.BareArg, tokenBuilder.ToString());
				case ScannerState.InShortArg:
					return new Token(TokenType.ShortOption, tokenBuilder.ToString());
				case ScannerState.InLongArg:
					return new Token(TokenType.LongOption, tokenBuilder.ToString());
				default:
					throw new Exception(string.Format("Unexpected end of arg while parsing {0}.", state.ToString()));
			}
		}

		public static List<Token> TokenizeArgs(string[] args) {
			List<Token> tokens = new List<Token>();
			foreach (string arg in args) {
				int idx = 0;

				while (idx < arg.Length) {
					Token token = NextTokenFromArg(arg, ref idx);

					switch (state) {
						case ScannerState.NeedsNextBare:
						case ScannerState.NeedsNextQuote:
							break;
						default:
							tokens.Add(token);
							break;
					}
				}

				switch (state) {
					case ScannerState.InShortArg:
						state = ScannerState.Unstarted;
						break;
					default:
						break;
				}
			}

			return tokens;
		}
	}
}

