using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ArgParseSharp
{
	public enum ArgumentType : byte {
		Positional,
		Named
	}

	public enum ArgumentStatus : byte {
		Required,
		Optional
	}

	public class ArgumentDef {
		public const char nullChar = '\0';
		public const string emptyString = "";

		public char ShortName;
		public string LongName;
		public ArgumentType ArgType;
		public ArgumentStatus ArgStatus;
		/// <summary>
		/// The number of following arguments to consume, as a signed byte.  Negative values imply "greedy", meaning the
		/// argument will consume all following sequential bare words.  Default is 0 (denoting a boolean flag).
		/// </summary>
		public sbyte Consumption;

		public Type DataType
		{
			get {
				return _dataType;
			}
			set {
				switch (Type.GetTypeCode(value)) {
					case TypeCode.String:
					case TypeCode.Boolean:
					case TypeCode.SByte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Byte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Object:
						_dataType = value;
						break;
					default:
						throw new NotImplementedException(string.Format(
							"Cannot set DataType to type '{0}': Unsupported type.",
							value.FullName
						));
				}
			}
		}

		private Type _dataType;

		public ArgumentDef(
			char shortName,
			string longName,
			Type dataType = null,
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) {
			if (shortName == nullChar && longName == emptyString) {
				throw new Exception("Argument must have a ShortName, LongName, or both.");
			}

			if (argType == ArgumentType.Positional && consumption == 0) {
				throw new Exception("Invalid number of values for positional argument '{0}'.  Positional arguments must be represented by at least one value.");
			}

			if (dataType == null) {
				dataType = typeof(object);
			}

			this._dataType = null;
			this.ShortName = shortName;
			this.LongName = longName;
			this.ArgType = argType;
			this.ArgStatus = argStatus;
			this.Consumption = consumption;

			this.DataType = dataType;
		}

		public ArgumentDef(
			char shortName,
			Type dataType = null,
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) : this(shortName, emptyString, dataType, argType, argStatus, consumption) {
		}

		public ArgumentDef(
			string longName,
			Type dataType = null,
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) : this(nullChar, longName, dataType, argType, argStatus, consumption) {
		}

		public string HumanName() {
			if (LongName != emptyString) {
				return LongName;
			}
			else {
				return ShortName.ToString();
			}
		}

		public override string ToString() {
			return string.Format(
				"[ArgumentDef(shortName: '{0}', longName: \"{1}\", dataType: {2}, argType: {3}, argStatus: {4}, consumption: {5})]",
				ShortName == nullChar ? "\\0" : ShortName.ToString(),
				LongName,
				DataType.FullName,
				ArgType.ToString(),
				ArgStatus.ToString(),
				Consumption
			);
		}
	}

	public struct Argument {
		public ArgumentDef Definition;
		public object Value;

		public override string ToString() {
			string valueString;

			if (Value is IList) {
				valueString = Utils.JoinArray(Value as IList);
			}
			else {
				valueString = Utils.SafeFormatObject(Value, string.Empty);
			}

			return string.Format("[Argument(definition: {0}, value: {1})]", Definition.ToString(), valueString);
		}
	}

	public interface ArgumentCollection : IEnumerable<KeyValuePair<ArgumentDef, Argument>> {
		void AddArgumentByDef(ArgumentDef key, Argument value);

		Argument GetArgument(ArgumentDef def);

		Argument GetArgumentByName(char shortName);

		Argument GetArgumentByName(string longName);

		bool ContainsArgument(ArgumentDef def);

		bool ContainsArgumentByName(char shortName);

		bool ContainsArgumentByName(string longName);

		Argument this[ArgumentDef def] { get; }

		Argument this[char shortName] { get; }

		Argument this[string longName] { get; }
	}

	public static class ArgParser {
		private class _ArgumentCollection : ArgumentCollection, IEnumerable<KeyValuePair<ArgumentDef, Argument>> {
			private Dictionary<ArgumentDef, Argument> args;

			public void AddArgumentByDef(ArgumentDef key, Argument value) {
				if (args.ContainsKey(key)) { // This would probably signify a bug in the parser.
					throw new Exception(string.Format("Cannot add duplicate argument {0}", key.HumanName()));
				}
				args[key] = value;
			}

			public Argument GetArgument(ArgumentDef def) {
				return args[def];
			}

			public Argument GetArgumentByName(char shortName) {
				return args[ArgParser.GetDefByShortName(shortName)];
			}

			public Argument GetArgumentByName(string longName) {
				return args[ArgParser.GetDefByLongName(longName)];
			}

			public bool ContainsArgument(ArgumentDef def) {
				return args.ContainsKey(def);
			}

			public bool ContainsArgumentByName(char shortName) {
				return args.ContainsKey(ArgParser.GetDefByShortName(shortName));
			}

			public bool ContainsArgumentByName(string longName) {
				return args.ContainsKey(ArgParser.GetDefByLongName(longName));
			}

			public Argument this[ArgumentDef def]
			{
				get {
					return GetArgument(def);
				}
			}

			public Argument this[char shortName]
			{
				get {
					return GetArgumentByName(shortName);
				}
			}

			public Argument this[string longName]
			{
				get {
					return GetArgumentByName(longName);
				}
			}

			public IEnumerator<KeyValuePair<ArgumentDef, Argument>> GetEnumerator() {
				return args.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			internal _ArgumentCollection(Dictionary<ArgumentDef, Argument> _args) {
				args = _args;
			}

			internal _ArgumentCollection() : this(new Dictionary<ArgumentDef, Argument>()) {
			}
		}

		public static IList<ArgumentDef> Defs { get; private set; }

		public static ArgumentCollection Arguments { get; private set; }

		private static List<ArgumentDef> defs;
		private static Dictionary<char, int> defsByShort;
		private static Dictionary<string, int> defsByLong;
		private static List<ArgumentDef> positionalDefs;
		private static List<ArgumentDef> requiredDefs;
		private static bool optionalPositionalDefined;
		private static bool greedyArgDefined;

		private static Dictionary<ArgumentDef, Argument> arguments;

		private static sbyte positionalIdx;

		static ArgParser() {
			defs = new List<ArgumentDef>();
			defsByShort = new Dictionary<char, int>();
			defsByLong = new Dictionary<string, int>();
			positionalDefs = new List<ArgumentDef>();
			requiredDefs = new List<ArgumentDef>();
			optionalPositionalDefined = false;
			greedyArgDefined = false;

			Defs = defs.AsReadOnly();

			arguments = new Dictionary<ArgumentDef, Argument>();
			Arguments = new _ArgumentCollection(arguments);

			positionalIdx = 0;
		}

		public static ArgumentDef AddArgument(
			char shortName,
			string longName,
			Type dataType = default(Type),
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) {
			ArgumentDef def = new ArgumentDef(shortName, longName, dataType, argType, argStatus, consumption);

			int _;

			if (def.ShortName != ArgumentDef.nullChar && defsByShort.TryGetValue(def.ShortName, out _)) {
				throw new Exception(string.Format("Cannot add new argument with duplicate short name '{0}'", def.ShortName));
			}

			if (def.LongName != ArgumentDef.emptyString && defsByLong.TryGetValue(def.LongName, out _)) {
				throw new Exception(string.Format("Cannot add new argument with duplicate long name \"{0}\"", def.LongName));
			}

			if (def.ArgType == ArgumentType.Positional && def.ArgStatus == ArgumentStatus.Required && optionalPositionalDefined) {
				throw new Exception("Cannot add new required positional arguments after optional positional arguments.");
			}

			if (def.ArgType == ArgumentType.Positional && def.ArgStatus == ArgumentStatus.Required && greedyArgDefined) {
				throw new Exception("Cannot add new required positional arguments after greedy arguments.");
			}

			defs.Add(def);

			if (def.Consumption < 0) {
				greedyArgDefined = true;
			}

			if (def.ArgType == ArgumentType.Positional) {
				positionalDefs.Add(def);

				if (def.ArgStatus == ArgumentStatus.Optional) {
					optionalPositionalDefined = true;
				}
			}

			if (def.ArgStatus == ArgumentStatus.Required) {
				requiredDefs.Add(def);
			}

			if (def.ShortName != ArgumentDef.nullChar) {
				defsByShort[def.ShortName] = defs.Count - 1;
			}

			if (def.LongName != ArgumentDef.emptyString) {
				defsByLong[def.LongName] = defs.Count - 1;
			}

			return def;
		}

		public static ArgumentDef AddArgument(
			char shortName,
			Type dataType = default(Type),
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) {
			return AddArgument(shortName, ArgumentDef.emptyString, dataType, argType, argStatus, consumption);
		}

		public static ArgumentDef AddArgument(
			string longName,
			Type dataType = default(Type),
			ArgumentType argType = ArgumentType.Positional,
			ArgumentStatus argStatus = ArgumentStatus.Required,
			sbyte consumption = 0
		) {
			return AddArgument(ArgumentDef.nullChar, longName, dataType, argType, argStatus, consumption);
		}

		public static ArgumentDef GetDefByShortName(char shortName) {
			int idx;

			if (defsByShort.TryGetValue(shortName, out idx)) {
				return defs[idx];
			}

			throw new Exception(string.Format("No argument found with short name '{0}'", shortName));
		}

		public static ArgumentDef GetDefByLongName(string longName) {
			int idx;

			if (defsByLong.TryGetValue(longName, out idx)) {
				return defs[idx];
			}

			throw new Exception(string.Format("No argument found with long name '{0}'", longName));
		}

		private static ArgumentDef GetDefByToken(Token token) {
			switch (token.Type) {
				case TokenType.ShortOption:
					return GetDefByShortName((char)token.Data);
				case TokenType.LongOption:
					return GetDefByLongName((string)token.Data);
				default:
					throw new NotImplementedException();
			}
		}

		private static ArgumentDef GetNextPositionalDef() {
			if (positionalDefs.Count > positionalIdx) {
				return positionalDefs[positionalIdx++];
			}
			throw new Exception(string.Format("Too many positional arguments; expected no more than {0}.", positionalDefs.Count));
		}

		private static List<ArgumentDef> FindMissingRequiredDefs() {
			List<ArgumentDef> missingDefs = new List<ArgumentDef>();
			foreach (ArgumentDef def in requiredDefs) {
				Argument _;
				if (!arguments.TryGetValue(def, out _)) {
					missingDefs.Add(def);
				}
			}

			return missingDefs;
		}

		public static ArgumentCollection ParseArgs(string[] args) {
			List<Token> tokens = ArgScanner.TokenizeArgs(args);

			IEnumerator<Token> tokenIterator = tokens.GetEnumerator();

			while (tokenIterator.MoveNext()) {
				Argument arg = ParseNextToken(tokenIterator);
				arguments.Add(arg.Definition, arg);
			}

			List<ArgumentDef> missingDefs = FindMissingRequiredDefs();

			if (missingDefs.Count > 0) {
				throw new Exception("Missing required arguments");
			}

			return Arguments;
		}

		private static Argument ParseNextToken(IEnumerator<Token> tokenIterator) {
			Token token = tokenIterator.Current;

			switch (token.Type) {
				case TokenType.ShortOption:
					return ParseShortArg(tokenIterator);
				case TokenType.LongOption:
					return ParseLongArg(tokenIterator);
				case TokenType.BareArg:
					return ParsePositionalArg(tokenIterator);
				default:
					throw new NotImplementedException("We don't serve your kind here");
			}
		}

		private static Argument ParsePositionalArg(IEnumerator<Token> tokenIterator) {
			ArgumentDef def = GetNextPositionalDef();
			Argument arg = new Argument();
			arg.Definition = def;


			sbyte iter = 0;

			List<object> argValues = new List<object>();

			do {
				iter++;
				Token token = tokenIterator.Current;

				if (token.Type != TokenType.BareArg) {
					return arg;
				}

				argValues.Add(ConvertTokenDataToType(token.Data, def.DataType));
			} while ((def.Consumption < 0 || (iter < def.Consumption)) && tokenIterator.MoveNext());

			if (argValues.Count < def.Consumption) {
				throw new Exception(string.Format(
					"Not enough values for positional argument '{0}'; {1} {2} required.",
					def.ToString(),
					def.Consumption,
					def.Consumption > 1 ? "are" : "is"
				));
			}

			if (def.Consumption == 1) {
				arg.Value = argValues[0];
			}
			else {
				arg.Value = argValues;
			}

			return arg;
		}

		private static Argument ParseLongArg(IEnumerator<Token> tokenIterator) {
			Token token = tokenIterator.Current;

			ArgumentDef def = GetDefByToken(token);

			Argument arg = new Argument();
			arg.Definition = def;

			if (def.Consumption == 0) { // No consumption
				return arg;
			}
			else if (def.Consumption == 1) { // Consume single following argument
				if (tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string data;

					// If the next token is a ShortOption but we're a consumer, grab new bare data out of the
					// next sequential ShortOptions
					switch (next.Type) {
						case TokenType.BareArg:
							data = (string)next.Data;
							break;
						default:
							throw new Exception(string.Format(
								"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
								next.Type,
								token.Type,
								Utils.JoinArray(new TokenType[] {
									TokenType.BareArg
								})
							));
					}

					arg.Value = ConvertTokenDataToType(data, def.DataType);
					return arg;
				}

				throw new Exception(string.Format(
					"No arguments after option '{0}'; {1} {2} required.",
					def.ToString(),
					def.Consumption,
					def.Consumption > 1 ? "are" : "is"
				));
			}
			else if (def.Consumption < 0) { // Greedy
				bool firstTime = true;
				List<object> argData = new List<object>();

				while (tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string tokenData;

					switch (next.Type) {
						case TokenType.BareArg:
							tokenData = (string)next.Data;
							break;
						default:
							if (firstTime) {
								throw new Exception(string.Format(
									"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
									next.Type,
									token.Type,
									Utils.JoinArray(new TokenType[] {
										TokenType.ShortOption,
										TokenType.BareArg
									})
								));
							}
							else {
								arg.Value = argData;
								return arg;
							}
					}

					argData.Add(ConvertTokenDataToType(tokenData, def.DataType));
				}

				if (argData.Count < 1) {
					throw new Exception(string.Format(
						"No arguments after greedy option '{0}'; at least one is required.",
						def.ToString()
					));
				}
				arg.Value = argData;
				return arg;
			}
			else { // Fixed, positive number of args.
				bool firstTime = true;
				List<object> argData = new List<object>();

				sbyte iter = 0;
				while (iter++ < def.Consumption && tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string tokenData;

					switch (next.Type) {
						case TokenType.BareArg:
							tokenData = (string)next.Data;
							break;
						default:
							if (firstTime) {
								throw new Exception(string.Format(
									"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
									next.Type,
									token.Type,
									Utils.JoinArray(new TokenType[] {
										TokenType.ShortOption,
										TokenType.BareArg
									})
								));
							}
							else {
								arg.Value = argData;
								return arg;
							}
					}

					argData.Add(ConvertTokenDataToType(tokenData, def.DataType));
				}

				if (argData.Count < def.Consumption) {
					throw new Exception(string.Format(
						"Not enough arguments after option '{0}'; {1} {2} required.",
						def.ToString(),
						def.Consumption,
						def.Consumption > 1 ? "are" : "is"
					));
				}
				arg.Value = argData;
				return arg;
			}
		}

		private static Argument ParseShortArg(IEnumerator<Token> tokenIterator) {
			Token token = tokenIterator.Current;

			ArgumentDef def = GetDefByToken(token);

			Argument arg = new Argument();
			arg.Definition = def;

			if (def.Consumption == 0) { // No consumption
				return arg;
			}
			else if (def.Consumption == 1) { // Consume single following argument
				if (tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string data;

					// If the next token is a ShortOption but we're a consumer, grab new bare data out of the
					// next sequential ShortOptions
					switch (next.Type) {
						case TokenType.ShortOption:
							data = ConsumeSequentialShortOptions(tokenIterator);
							break;
						case TokenType.BareArg:
							data = (string)next.Data;
							break;
						default:
							throw new Exception(string.Format(
								"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
								next.Type,
								token.Type,
								Utils.JoinArray(new TokenType[] {
									TokenType.BareArg
								})
							));
					}

					arg.Value = ConvertTokenDataToType(data, def.DataType);
					return arg;
				}

				throw new Exception(string.Format(
					"No arguments after option '{0}'; {1} {2} required.",
					def.ToString(),
					def.Consumption,
					def.Consumption > 1 ? "are" : "is"
				));
			}
			else if (def.Consumption < 0) { // Greedy
				bool firstTime = true;
				List<object> argData = new List<object>();

				while (tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string tokenData;

					switch (next.Type) {
						case TokenType.ShortOption:
							if (firstTime) {
								tokenData = ConsumeSequentialShortOptions(tokenIterator);
							}
							else {
								arg.Value = argData;
								return arg;
							}
							break;
						case TokenType.BareArg:
							tokenData = (string)next.Data;
							break;
						default:
							if (firstTime) {
								throw new Exception(string.Format(
									"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
									next.Type,
									token.Type,
									Utils.JoinArray(new TokenType[] {
										TokenType.ShortOption,
										TokenType.BareArg
									})
								));
							}
							else {
								arg.Value = argData;
								return arg;
							}
					}

					argData.Add(ConvertTokenDataToType(tokenData, def.DataType));
				}

				if (argData.Count < 1) {
					throw new Exception(string.Format(
						"No arguments after greedy option '{0}'; at least one is required.",
						def.ToString()
					));
				}
				arg.Value = argData;
				return arg;
			}
			else { // Fixed, positive number of args.
				bool firstTime = true;
				List<object> argData = new List<object>();

				sbyte iter = 0;
				while (iter++ < def.Consumption && tokenIterator.MoveNext()) {
					Token next = tokenIterator.Current;
					string tokenData;

					switch (next.Type) {
						case TokenType.ShortOption:
							if (firstTime) {
								tokenData = ConsumeSequentialShortOptions(tokenIterator);
							}
							else {
								arg.Value = argData;
								return arg;
							}
							break;
						case TokenType.BareArg:
							tokenData = (string)next.Data;
							break;
						default:
							if (firstTime) {
								throw new Exception(string.Format(
									"Unexpected token type '{0}' after '{1}': Expected one of '{2}'",
									next.Type,
									token.Type,
									Utils.JoinArray(new TokenType[] {
										TokenType.ShortOption,
										TokenType.BareArg
									})
								));
							}
							else {
								if (argData.Count < def.Consumption) {
									throw new Exception(string.Format(
										"Not enough arguments after option '{0}'; {1} {2} required.",
										def.ToString(),
										def.Consumption,
										def.Consumption > 1 ? "are" : "is"
									));
								}

								arg.Value = argData;
								return arg;
							}
					}

					argData.Add(ConvertTokenDataToType(tokenData, def.DataType));
				}

				if (argData.Count < def.Consumption) {
					throw new Exception(string.Format(
						"Not enough arguments after option '{0}'; {1} {2} required.",
						def.ToString(),
						def.Consumption,
						def.Consumption > 1 ? "are" : "is"
					));
				}

				arg.Value = argData;
				return arg;
			}
		}

		private static string ConsumeSequentialShortOptions(IEnumerator<Token> tokenIterator) {
			Token token = tokenIterator.Current;

			if (token.Type != TokenType.ShortOption) {
				throw new ArgumentException("Argument must point to a valid ShortOption token.", "tokenIterator");
			}

			StringBuilder sb = new StringBuilder();

			sb.Append((char)token.Data);

			while (tokenIterator.MoveNext()) {
				token = tokenIterator.Current;

				if (token.Type == TokenType.ShortOption) {
					sb.Append((char)token.Data);
				}
				else {
					break;
				}
			}

			return sb.ToString();
		}

		private static object ConvertTokenDataToType(object data, Type t) {
			switch (Type.GetTypeCode(t)) {
				case TypeCode.String:
					return (string)data;
				case TypeCode.Boolean:
					return Boolean.Parse((string)data);
				case TypeCode.SByte:
					return SByte.Parse((string)data);
				case TypeCode.Int16:
					return Int16.Parse((string)data);
				case TypeCode.Int32:
					return Int32.Parse((string)data);
				case TypeCode.Int64:
					return Int64.Parse((string)data);
				case TypeCode.Byte:
					return Byte.Parse((string)data);
				case TypeCode.UInt16:
					return UInt16.Parse((string)data);
				case TypeCode.UInt32:
					return UInt32.Parse((string)data);
				case TypeCode.UInt64:
					return UInt64.Parse((string)data);
				case TypeCode.Single:
					return Single.Parse((string)data);
				case TypeCode.Double:
					return Double.Parse((string)data);
				case TypeCode.Object:
					return data;
				default:
					throw new NotImplementedException(string.Format(
						"Cannot convert data to type '{0}': Unsupported type.",
						t.FullName
					));
			}
		}

		private static T ConvertTokenDataToType<T>(object data) {
			return (T)ConvertTokenDataToType(data, typeof(T));
		}
	}
}

