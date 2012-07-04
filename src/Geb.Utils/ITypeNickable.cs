using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public interface ITypeNickable
    {
        /// <summary>
        /// 类型的昵称，用于一些特殊的场合，如：
        ///     假设类型被混淆过，TypeNickName 可以用来表示这个类型。或其它用途。
        /// </summary>
        String TypeNickName { get; }
    }
}
