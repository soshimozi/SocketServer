using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateJavaClass
{
    class JavaBuilder
    {
        private StringBuilder _builder = new StringBuilder();

        public void StartClass(string className)
        {
            _builder.AppendFormat("public class {0} {{\r\n", className);
        }

        public void AddField(string typeName, string fieldName)
        {
            _builder.AppendFormat("\tprivate {0} {1};\r\n", typeName, fieldName);
        }

        public void AddGetter(string typeName, string fieldName)
        {
            string getterMethodName = Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            _builder.AppendFormat("\tpublic {1} get{0}() {{\r\n\t\treturn this.{2};\r\n\t}}\r\n", getterMethodName, typeName, fieldName);
        }

        public void EndClass()
        {
            _builder.Append("}\r\n");
        }

        public override string ToString()
        {
            return _builder.ToString();
        }


        internal void AddSetter(string typeName, string fieldName)
        {
            string setterMethodName = Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            _builder.AppendFormat("\tpublic void set{0}({1} value) {{\r\n\t\tthis.{2} = value;\r\n\t}}\r\n", setterMethodName, typeName, fieldName);
        }

        internal void AddPackageName(string packageName)
        {
            _builder.AppendFormat("package {0};\r\n", packageName);
        }

        internal void AddImport(string referenceName)
        {
            _builder.AppendFormat("import {0};\r\n", referenceName);
        }
    }
}
