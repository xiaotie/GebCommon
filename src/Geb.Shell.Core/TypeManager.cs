/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geb.Shell.Core
{
    /// <summary>
    /// TypeDictionary 之间组成一个树状结构
    /// 
    /// TypeDictionary(Root)
    ///     |--TypeDictionary
    ///     |       |--TypeDictionary
    ///     |       |--TypeDictionary
    ///     |       |......
    ///     |       |--Type
    ///     |       |--Type
    ///     |       |......
    ///     |
    ///     |--TypeDictionary
    ///     |......
    ///     |--Type
    ///     |--Type
    ///     |......
    ///     
    /// </summary>
    public class TypeManager
    {
        public TypeDictionary Root;

        public TypeDictionary Now;

        public TypeManager()
        {
            Root = new TypeDictionary("Root");
            Root.Parent = Root;
            Now = Root;
        }

        public void AddType(Type type)
        {
            String[] nameList = type.FullName.Split('.');

            TypeDictionary parent = Root;

            if (nameList.Length > 0)
            {
                String fullName = String.Empty;

                for (int i = 0; i < (nameList.Length - 1); i++)
                {
                    if (fullName != String.Empty)
                    {
                        fullName += ".";
                    }

                    String name = nameList[i];

                    fullName += name;

                    if (parent.SubTypeDictionary.ContainsKey(name))
                    {
                        parent = parent.SubTypeDictionary[name];
                    }
                    else
                    {
                        TypeDictionary dic = new TypeDictionary(name);
                        dic.FullName = fullName;
                        dic.Parent = parent;
                        parent.SubTypeDictionary.Add(name, dic);
                        parent = dic;
                    }
                }

                String typeName = nameList[nameList.Length - 1];

                if (!parent.Types.ContainsKey(typeName))
                {
                    parent.Types.Add(typeName, type);
                }
            }
        }

        public void StepUp()
        {
            if (Now == null) return;
            Now = Now.Parent;
        }

        public void StepDown(String dir)
        {
            if (Now == null) return;
            if (Now.SubTypeDictionary.ContainsKey(dir))
            {
                Now = Now.SubTypeDictionary[dir];
            }
        }
    }
}
