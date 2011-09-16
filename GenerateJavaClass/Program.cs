using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace GenerateJavaClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run(args);
        }

        public void Run(string[] args)
        {
            if (args.Count() < 1 || args.Count() > 2)
            {
                PrintUsage();
                return;
            }

            string typeName = string.Empty;
            string executableName = string.Empty;

            if (args.Count() == 1)
            {
                executableName = args[0];
            }
            else
            {
                typeName = args[0];
                executableName = args[1];
            }

            GenerateClass(typeName, executableName);

        }

        private void GenerateClass(string typeName, string executableName)
        {
            Assembly assembly = null;

            try
            {
                string executablePath
                    = Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        executableName);

                assembly = Assembly.LoadFile(executablePath);
            }
            catch (BadImageFormatException bi)
            {
                Console.WriteLine(bi.Message);
            }
            catch (System.IO.FileNotFoundException fi)
            {
                Console.WriteLine(fi.Message);
            }
            catch(System.IO.FileLoadException fl)
            {
                Console.WriteLine(fl.Message);
            }

            if (assembly != null)
            {
                if (!string.IsNullOrEmpty(typeName))
                {
                    //Type type = assembly.GetType(typeName, false);
                    //if (type != null)
                    //{
                    //    WriteJavaClass(type.Name, type);
                    //}
                    //else
                    //{
                    //    // Handle error
                    //}
                }
                else
                {
                    // iterate each type in assembly
                    // if type is not static, abstract, and not an interface
                    // generate the type

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsAbstract && type.IsClass && type.IsPublic)
                        {
                            GetAssemblyName(assembly);
                            WriteJavaClass(assembly, type);
                        }
                    }
                }
                
            }

        }

        private string GetAssemblyName(Assembly assembly)
        {
            string fullName = assembly.FullName;

            string[] nameSections = fullName.Split(",".ToCharArray());

            return nameSections[0];
        }

        private void WriteJavaClass(Assembly assembly, Type type)
        {
            FileStream fs = File.OpenWrite(type.Name + ".java");
            string java = GenerateJavaClassFromAssemblyType(assembly, type);
            fs.Write(ASCIIEncoding.ASCII.GetBytes(java.ToCharArray()), 0,
                    ASCIIEncoding.ASCII.GetByteCount(java.ToCharArray()));
            fs.Close();
        }

        private string GenerateJavaClassFromAssemblyType(Assembly assembly, Type type)
        {
            JavaBuilder builder = new JavaBuilder();

            string className = type.Name;

            string assemblyName = GetAssemblyName(assembly);

            builder.AddPackageName(assemblyName);

            // get references for imports
            //AssemblyName[] references = assembly.GetReferencedAssemblies();
            //foreach (AssemblyName name in references)
            //{
            //    builder.AddImport(name.Name);
            //}

            builder.StartClass(className);

            List<TypeFieldPair> fieldPairs = new List<TypeFieldPair>();

            // get fields
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                string typeName = GetJavaTypeName(field.FieldType);
                builder.AddField(typeName, field.Name);
                fieldPairs.Add(new TypeFieldPair() { FieldName = field.Name, FieldType = field.FieldType });
            }

            // generate getters/setters
            foreach (TypeFieldPair fieldPair in fieldPairs)
            {
                string typeName = GetJavaTypeName(fieldPair.FieldType);
                builder.AddGetter(typeName, fieldPair.FieldName);
                builder.AddSetter(typeName, fieldPair.FieldName);
            }

            builder.EndClass();

            return builder.ToString();
        }

        private string GetJavaTypeName(Type type)
        {
            string javaType = "";

            if (type.IsArray)
            {
                if (type.HasElementType)
                {
                    Type elementType = type.GetElementType();

                    javaType = GetJavaTypeName(elementType) + "[]";
                }

            } else if(type.IsValueType)
            {
                javaType = ValueTypeToJavaType(type);
            }
            else
            {
                javaType = type.Name;
            }


            return javaType;
        }

        private string ValueTypeToJavaType(Type type)
        {
            string javaType = "";

            if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32))
            {
                javaType = "int";
            }
            else if (type == typeof(long) || type == typeof(Int64))
            {
                javaType = "long";
            }
            else if (type == typeof(Guid))
            {
                javaType = "String";
            }
            else if (type == typeof(double))
            {
                javaType = "double";
            }
            else if (type == typeof(float))
            {
                javaType = "float";
            }
            else if (type == typeof(byte))
            {
                javaType = "byte";
            }
            else if (type == typeof(char))
            {
                javaType = "char";
            }
            else if (type == typeof(bool) || type == typeof(Boolean))
            {
                javaType = "boolean";
            }


            return javaType;
        }

        private void PrintUsage()
        {
            Console.WriteLine("Usage: GenerateJavaClass [type name] Executable");
        }

        private class TypeFieldPair
        {
            public Type FieldType;
            public string FieldName;
        }
    }
}
