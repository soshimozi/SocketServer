using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
using System.IO;

/**
 * This is a simple XML to object de-serializer. The SimpleReader class
 * extends ObjectReader. It reads an object from a XmlReader object
 * and puts it back to a live C# object. The XML representation of the
 * object is a standard WOX representation. For more information about
 * the XML representation please visit: http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: SimpleReader.cs - 1.0
 */
namespace wox.serial
{
    public class SimpleReader : ObjectReader
    {
        private static ILog log = LogManager.GetLogger(typeof(SimpleReader));

        private Dictionary<string, Object> map;

        public SimpleReader()
        {
            map = new Dictionary<string, object>();
        }

        //public override Object read(byte [] data)
        //{
        //    String xmlString = string.Empty;

        //    try
        //    {
        //        xmlString = ASCIIEncoding.ASCII.GetString(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //    }

        //    Object value = null;

        //    if (element.HasAttributes || !element.IsEmpty)
        //    {
        //        if (element.GetAttribute(IDREF) != null)
        //        {
        //            // get referenced object
        //            if (map.ContainsKey(element.GetAttribute(IDREF)))
        //            {
        //                value = map[element.GetAttribute(IDREF)];
        //            }
        //        }
        //        else
        //        {
        //            // at this point we must be reading an actual Object
        //            // so we need to store it in
        //            // there are two ways we can handle objects referred to
        //            // by idrefs
        //            // the  simplest is to put all objects in an ArrayList or
        //            // HashMap, and then get retrieve the objects from the collection
        //            value = readObject(element);
        //        }
        //    }

        //    return value;
        //}

        //private object readObject(XmlElement element)
        //{
        //    object value = null;
        //    String objectId = element.GetAttribute(ID);
        //    if (!string.IsNullOrEmpty(objectId))
        //    {
        //    }

        //    return value;

        //}

        private ObjectXml DeserializeIntermediateObject(string xml)
        {
            try
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(ObjectXml));
                MemoryStream ms = new MemoryStream(ASCIIEncoding.Unicode.GetBytes(xml));
                return (ObjectXml)deserializer.Deserialize(ms);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return null;
            }
        }

        public override Object read(XmlReader xob)
        {
            // there are several possibilities - see how we handle them

            if (empty(xob))
            {
                return null;
            } else if (reference(xob))
            {
                //System.out.println("it is a reference: " + xob.getAttributeValue(IDREF) );
                //printMap();
                //return map.get(xob.getAttributeValue(IDREF));
                return map[xob.GetAttribute(IDREF)];
            }

            // at this point we must be reading an actual Object
            // so we need to store it in
            // there are two ways we can handle objects referred to
            // by idrefs
            // the  simplest is to put all objects in an ArrayList or
            // HashMap, and then get retrieve the objects from the collection
            Object ob = null;
            String id = xob.GetAttribute(ID);
            //Console.Out.WriteLine("id: " + id);
            if (isPrimitiveArray(xob))
            {
                //Console.Out.WriteLine("readPrimitiveArray: " + xob.GetAttribute(TYPE));
                ob = readPrimitiveArray(xob, id);
                return ob;
            }

            else if (isObjectArray(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                //Console.Out.WriteLine("readObjectArray: " + xob.GetAttribute(TYPE));
                ob = readObjectArray(xob, id);
                //Console.Out.WriteLine("ready to return array");
                return ob;
            }
            else if (isArrayList(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                ob = readArrayList(xob, id);
            }
            else if (isMap(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                ob = readMap(xob, id);
            }
            else if (Util.stringable(xob.GetAttribute(TYPE)))
            {
                //Console.Out.WriteLine("it is an stringable object!!!...");
                //System.out.println("readStringObject: " + xob.getAttributeValue(TYPE));
                ob = readStringObject(xob, id);
                return ob;
            }
            else
            { // assume we have a normal object with some fields to set
                //System.out.println("readObject: " + xob.getAttributeValue(TYPE));
                //Console.Out.WriteLine("it is a NORMAL object!!!...");
                ob = readObject(xob, id);
            }
            // now place the object in a collection for later reference
            //System.out.println("ob: " + ob + ", id: " + id);
            return ob;
        }

        //public boolean empty(Element xob)
        public bool empty(XmlReader xob)
        {
            // empty only if it has no attributes and no content
            // System.out.println("Empty test on: " + xob);
            //return !xob.getAttributes().iterator().hasNext() &&
            //!xob.getContent().iterator().hasNext();
            return !xob.HasAttributes && xob.IsEmptyElement;
        }

        //public bool reference(Element xob)
        public bool reference(XmlReader xob)
        {
            //            bool ret = xob.getAttribute(IDREF) != null;
            bool ret = xob.GetAttribute(IDREF) != null;
            // System.out.println("Reference? : " + ret);
            return ret;
        }

        //public boolean primitiveArray(Element xob)
        public bool isPrimitiveArray(XmlReader xob)
        {
            //if (!xob.Name.Equals(ARRAY))
            if (!xob.GetAttribute(TYPE).Equals(ARRAY))
            {
                //Console.Out.WriteLine("Name of the element is not array");
                return false;
            }
            // at this point we must have an array - but is it
            // primitive?  - iterate through all the primitive array types to see
            //String arrayType = xob.GetAttribute(TYPE);
            String arrayType = xob.GetAttribute(ELEMENT_TYPE);
            for (int i = 0; i < primitiveArraysWOX.Length; i++)
            {
                if (primitiveArraysWOX[i].Equals(arrayType))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isObjectArray(XmlReader xob)
        {
            // this actually returns true for any array
            //return xob.Name.Equals(ARRAY);            
            return xob.GetAttribute(TYPE).Equals(ARRAY);
        }


        public bool isArrayList(XmlReader xob)
        {
            // this actually returns true for any arrayList
            return xob.GetAttribute(TYPE).Equals(ARRAYLIST);
        }


        public bool isMap(XmlReader xob)
        {
            // this actually returns true for any map (hashtable in C#)
            return xob.GetAttribute(TYPE).Equals("java.util.Hashtable");
        }


        //----------

        // now on to the reading methods
        public Object readPrimitiveArray(XmlReader xob, string id)
        {
            //Console.Out.WriteLine("inside readPrimitiveArray...");
            try
            {
                //Class type = getPrimitiveType(xob.getAttributeValue(TYPE));
                //get the Java type that corresponds to the WOX type
                Type arrayType = (Type)mapWOXToCSharp[xob.GetAttribute(ELEMENT_TYPE)];
                //Console.Out.WriteLine("arrayType: " + arrayType.ToString());
                //get the wrapper type to be able to construct the elements of the array
                //Class wrapperType = getWrapperType(type);
                //System.out.println("type: " + type + ", wrapperType: " + wrapperType);

                int len = Int32.Parse(xob.GetAttribute(LENGTH));
                //Console.Out.WriteLine("length: " + len);
                Array array = Array.CreateInstance(arrayType, len);
                //Console.Out.WriteLine("array created");

                //get array as string. e.g. "67 87 98 87"            
                String st = xob.ReadString();
                //Console.Out.WriteLine("st: " + st);
                char[] seps = { ' ' };
                //get the array elements in as an array of Strings
                String[] values = st.Split(seps);


                //code added by Carlos Jaimez (29th April 2005)
                /*if ((type.equals(byte.class)) || (type.equals(Byte.class))) {
                    Object byteArray = readByteArray(xob);
                    //if it is a Byte array, we have to copy the byte array into it
                    if (type.equals(Byte.class)) {
                        byte[] arrayPrimitiveByte = (byte[])byteArray;
                        Byte[] arrayWrapperByte = new Byte[arrayPrimitiveByte.length];
                        for(int k=0; k<arrayPrimitiveByte.length; k++){
                            arrayWrapperByte[k] = new Byte(arrayPrimitiveByte[k]);
                        }
                        map.put(id, arrayWrapperByte);
                        return arrayWrapperByte;
                    }
                    //if it is a byte array
                    else{
                        map.put(id, byteArray);
                        return byteArray;
                    }
                }*/

                //determine the type primitive type of the array
                if (arrayType.Equals(typeof(System.SByte)))
                {

                }
                else if (arrayType.Equals(typeof(System.Int16)))
                {
                    //Console.Out.WriteLine("array of Int16");
                    Object shortArray = readShortArray((short[])array, values);
                    map.Add(id, shortArray);
                    return shortArray;
                }
                else if (arrayType.Equals(typeof(System.Int32)))
                {
                    //Console.Out.WriteLine("array of Int32");
                    Object intArray = readIntArray((int[])array, values);
                    map.Add(id, intArray);
                    return intArray;
                }
                else if (arrayType.Equals(typeof(System.Int64)))
                {
                    //Console.Out.WriteLine("array of Int64");
                    Object longArray = readLongArray((long[])array, values);
                    map.Add(id, longArray);
                    return longArray;
                }
                else if (arrayType.Equals(typeof(System.Single)))
                {
                    //Console.Out.WriteLine("array of Float");
                    Object floatArray = readFloatArray((float[])array, values);
                    map.Add(id, floatArray);
                    return floatArray;
                }
                else if (arrayType.Equals(typeof(System.Double)))
                {
                    //Console.Out.WriteLine("array of Double");
                    Object doubleArray = readDoubleArray((double[])array, values);
                    map.Add(id, doubleArray);
                    return doubleArray;
                }
                else if (arrayType.Equals(typeof(System.Char)))
                {
                    //Console.Out.WriteLine("array of Char");
                    Object charArray = readCharArray((char[])array, values);
                    map.Add(id, charArray);
                    return charArray;
                }
                else if (arrayType.Equals(typeof(System.Boolean)))
                {
                    //Console.Out.WriteLine("array of Boolean");
                    Object boolArray = readBooleanArray((bool[])array, values);
                    map.Add(id, boolArray);
                    return boolArray;
                }
                else if (arrayType.Equals(typeof(System.Type)))
                {
                    Object classArray = readClassArray((Type[])array, values);
                    map.Add(id, classArray);
                    return classArray;
                }
                else
                {
                    return null;
                }

                map.Add(id, array);
                return array;

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("The exception is: " + e.Message);
                //e.printStackTrace();
                //throw new RuntimeException(e);
            }
            return "";
        }


        public Object readShortArray(short[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int16.Parse(s[i]);
            }
            return a;
        }

        public Object readIntArray(int[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int32.Parse(s[i]);
            }
            return a;
        }

        public Object readLongArray(long[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int64.Parse(s[i]);
            }
            return a;
        }

        public Object readFloatArray(float[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Single.Parse(s[i]);
            }
            return a;
        }

        public Object readDoubleArray(double[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Double.Parse(s[i]);
            }
            return a;
        }

        public Object readCharArray(char[] a, String[] s)
        {
            //Console.Out.WriteLine("reading array of char");
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                //the token represents the unicode value in the form "\\u0004"
                //Console.Out.WriteLine("s[" + i + "]:" + s[i]);                
                int decimalValue = getDecimalValue(s[i]);
                a[index++] = (char)decimalValue;
                //Console.Out.WriteLine(a[i]);
                //a[index++] = Double.Parse(s[i]);
            }
            return a;
        }

        public Object readBooleanArray(bool[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Boolean.Parse(s[i]);
            }
            return a;
        }

        private static int getDecimalValue(String unicodeValue)
        {
            //Console.Out.WriteLine("unicodeValue: " + unicodeValue);
            //first remove the "\\u" part of the unicode value
            //String unicodeModified = unicodeValue.substring(2, unicodeValue.length());
            String unicodeModified = unicodeValue.Substring(2, 4); //unicodeValue.Length);
            //Console.Out.WriteLine("unicodeModified: " + unicodeModified);
            //System.out.println("unicodeModified: " + unicodeModified);
            //int decimalValue = Int32.Parse(unicodeModified);
            int decimalValue = HexToInt(unicodeModified);
            //Console.Out.WriteLine("decimalValue:" + decimalValue);            
            return decimalValue;
        }

        //to convert an hexadecimal value to int
        private static int HexToInt(string hexString)
        {
            return int.Parse(hexString,
                System.Globalization.NumberStyles.HexNumber, null);
        }



        //-----------------------------------------------------------------------------
        /**
         * Purpose: To constuct the byte array based on the a and xob
         * Befor constructs back the byte array, it has to be decoded
         * Carlos Jaimez (29 april 2005)
         * @param a
         * @param xob
         * @return : int Array
         */
        /*public Object readByteArray(Element xob) {
            //get the encoded base64 text from the XML
            String strByte = xob.getText();
            //get the bytes from the string
            byte[] encodedArray = strByte.getBytes();
            System.out.println("encoded.length: " + encodedArray.length);
            //decode the source byte[] array
            byte[] decodedArray = EncodeBase64.decode(encodedArray);
            System.out.println("decoded.length: " + decodedArray.length);
            //return the real decoded array of byte
            return decodedArray;
        }*/




        public Object readClassArray(Type[] a, String[] s)
        {

            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                if (s[i].Equals("null"))
                {
                    a[index++] = null;
                }
                else
                {
                    Type cSharpClass = (Type)mapWOXToCSharp[s[i]];
                    //if the data type was NOT found in the map
                    if (cSharpClass == null)
                    {
                        cSharpClass = (Type)mapArrayWOXToCSharp[s[i]];
                        //if the data type was NOT found in the array map
                        if (cSharpClass == null)
                        {
                            try
                            {
                                //Console.Out.WriteLine("class NOT found in any of the maps: " + s[i]);
                                //a[index++] = Class.forName(s);
                                Type typeObject = Type.GetType(s[i]);
                                //Console.Out.WriteLine("typeObject: " + typeObject);
                                //checking if the type is not null
                                if (typeObject == null)
                                {
                                    //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                                    //try getting the type from the assembly that call this one...
                                    Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                    typeObject = MyAssembly.GetType(s[i]);
                                    //Console.Out.WriteLine("NOW typeObject: " + typeObject);
                                }
                                a[index++] = typeObject;
                            }
                            //catch(java.lang.ClassNotFoundException e){
                            catch (Exception e)
                            {
                                //e.printStackTrace();
                                Console.Out.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            //Console.Out.WriteLine("WOX type: " + s[i] + ", CSharp type (array): " + cSharpClass);
                            a[index++] = cSharpClass;
                        }
                    }
                    else
                    {
                        //Console.Out.WriteLine("WOX type: " + s[i] + ", CSharp type: " + cSharpClass);
                        a[index++] = cSharpClass;
                    }

                }

                //a[index++] = Boolean.Parse(s[i]);
            }
            return a;
        }


        public Object readMap(XmlReader xob, string id)
        {
            //Console.Out.WriteLine("Reading a Map...");
            Hashtable myHashtable = new Hashtable();

            //get the subtree of the MAP
            XmlReader xobSubTree = xob.ReadSubtree();
            //position the cursor at the beginning of the MAP <object type="map">
            xobSubTree.Read();
            //iterate all its entry objects <object type="entry">
            while (xobSubTree.Read())
            {
                //Console.Out.WriteLine("type subtree: " + xobSubTree.GetAttribute(TYPE));
                //we will only process Elements, and no EndElements
                if (xobSubTree.NodeType == XmlNodeType.Element)
                {
                    //Console.Out.WriteLine("about to read the node..." + xobSubTree.NodeType);
                    //Console.Out.WriteLine("this node contains: " + xob.ReadString());
                    //I will read the key
                    //XmlReader xobKey = xobSubTree.ReadSubtree();

                    //position the cursor in the KEY object
                    xobSubTree.Read();
                    //Console.Out.WriteLine("type key: " + xobSubTree.GetAttribute(TYPE));
                    //Read the KEY object
                    Object myKey = read(xobSubTree);
                    //position the cursor in the VALUE object
                    xobSubTree.Read();
                    //Console.Out.WriteLine("type value: " + xobSubTree.GetAttribute(TYPE));
                    //Read the VALUE object
                    Object myValue = read(xobSubTree);
                    //Add the key and the value to the Hashtable
                    myHashtable.Add(myKey, myValue);
                }
            }
            map.Add(id, myHashtable);
            return myHashtable;
        }


        public Object readArrayList(XmlReader xob, string id)
        {
            //Console.Out.WriteLine("Reading an ArrayList...");
            Array array = (Array)readObjectArrayGeneric(xob, id);
            ArrayList list = new ArrayList();
            //populate the ArrayList with the array elements
            for (int i = 0; i < array.GetLength(0); i++)
            {
                list.Add(array.GetValue(i));
            }
            map.Add(id, list);
            return list;
        }

        public Object readObjectArray(XmlReader xob, string id)
        {
            Object array = readObjectArrayGeneric(xob, id);
            map.Add(id, array);
            return array;
        }

        //-----------------------------------------------------------------------------
        public Object readObjectArrayGeneric(XmlReader xob, string id)
        {
            // to read an object array we first determine the
            // class of the array - leave this to a separate method
            // since there seems to be no automatic way to get the
            // type of the array

            //System.out.println("--------------------READ OBJECT ARRAY");
            try
            {
                //String arrayTypeName = xob.GetAttribute(TYPE);
                String arrayTypeName = xob.GetAttribute(ELEMENT_TYPE);
                int len = Int32.Parse(xob.GetAttribute(LENGTH));
                //Console.Out.WriteLine("type: " + arrayTypeName + ", len: " + len);
                Type componentType = getObjectArrayComponentType(arrayTypeName);
                //Console.Out.WriteLine("componentType: " + componentType);
                Array array = Array.CreateInstance(componentType, len);
                //Console.Out.WriteLine("array created...");
                //map.Add(id, array);
                // now fill in the array

                //get the subtree of the <array>
                XmlReader xobSubTree = xob.ReadSubtree();
                //position the cursor in the <array> element
                xobSubTree.Read();

                int index = 0;
                //for every node (element) in the array            
                while (xobSubTree.Read())
                {
                    //we will only process Elements, and no EndElements
                    if (xobSubTree.NodeType == XmlNodeType.Element)
                    {
                        //Console.Out.WriteLine("about to read the node..." + xobSubTree.NodeType);
                        //Console.Out.WriteLine("this node contains: " + xob.ReadString());
                        Object childArray = read(xobSubTree);
                        //Console.Out.WriteLine(index + " child: " + childArray);
                        array.SetValue(childArray, index++);
                    }
                }
                //Console.Out.WriteLine("returning array from readObjectArray");

                /*List children = xob.getChildren();
                int index = 0;
                for (Iterator i = children.iterator(); i.hasNext();) {
                    //System.out.println("before reading...");
                    Object childArray = read((Element) i.next());
                    //System.out.println(index + " child: " + childArray);
                    Array.set(array, index++, childArray);
                }*/

                return array;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception is: " + e.Message);
                //e.printStackTrace();
                //throw new RuntimeException(e);
                return null;
            }
        }

        public Type getObjectArrayComponentType(String arrayTypeName)
        {
            //throws Exception {
            // System.out.println("Getting class for: " + arrayTypeName);

            //we first look for the Java type in the map
            //Class javaClass = (Class)mapWOXToJava.get(arrayTypeName);
            Type cSharpClass = (Type)mapWOXToCSharp[arrayTypeName];
            //if the type was not found, we now look for it in the array map
            if (cSharpClass == null)
            {
                //Console.Out.WriteLine("cSharpClass was NULL... 1st time...");
                cSharpClass = (Type)mapArrayWOXToCSharp[arrayTypeName];
                //if the type is not in the array map
                if (cSharpClass == null)
                {
                    //Console.Out.WriteLine("WOX type not found in any of the maps...");
                    if (arrayTypeName.Equals("Object"))
                    {
                        //Console.Out.WriteLine("It is an array of Object...");
                        arrayTypeName = "System.Object";
                    }
                    Type typeObject = Type.GetType(arrayTypeName);
                    //Console.Out.WriteLine("typeObject: " + typeObject);
                    //checking if the type is not null
                    if (typeObject == null)
                    {
                        //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                        //try getting the type from the assembly that call this one...
                        Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                        typeObject = MyAssembly.GetType(arrayTypeName);
                        //Console.Out.WriteLine("NOW typeObject: " + typeObject);
                    }
                    return typeObject;
                }
                else
                {
                    //Console.Out.WriteLine("returning cSharpClass from mapArrayWOXToCSharp");
                    return cSharpClass;
                }
            }
            else
            {
                //Console.Out.WriteLine("returning cSharpClass from mapWOXToCSharp");
                return cSharpClass;
            }

            //        String componentTypeName = arrayTypeName.substring(1);
            //        System.out.println("Component type name: " + componentTypeName);
            //        Class componentType = Class.forName(componentTypeName);
            //        System.out.println("Component type: " + componentType);
            //        return componentType;
        }





        ///---------------
        ///


        public Object readStringObject(XmlReader xob, string id)
        {
            try
            {
                //get the CSharp type that corresponds to the WOX type in the XML
                Type cSharpType = (Type)mapWOXToCSharp[xob.GetAttribute(TYPE)];
                //Class type = Class.forName(xob.getAttributeValue(TYPE));
                // System.out.println("Declared: ");
                // print(type.getDeclaredConstructors());
                // System.out.println("All?: ");
                // print(type.getConstructors());
                // System.out.println("Type: " + type);
                // System.out.println("Text: " + xob.getText());
                // AccessController.doPrivileged(null);
                // PrivilegedAction action

                // handle class objects differently
                if (cSharpType.Equals(typeof(System.Type)))
                {
                    //look for the Java class that corresponds to the WOX type
                    Type cSharpClass = (Type)mapWOXToCSharp[xob.GetAttribute(VALUE)];
                    //if not found, look for it in the array map
                    if (cSharpClass == null)
                    {
                        cSharpClass = (Type)mapArrayWOXToCSharp[xob.GetAttribute(VALUE)];
                        //if not found, load it
                        if (cSharpClass == null)
                        {
                            //Console.Out.WriteLine("NOT found in any of the arrays: " + xob.GetAttribute(VALUE));
                            //Object obClass = Type.GetType(xob.GetAttribute(VALUE));

                            //Console.Out.WriteLine("type: " + xob.GetAttribute(VALUE));
                            Object obClass = Type.GetType(xob.GetAttribute(VALUE));
                            //Console.Out.WriteLine("obClass: " + obClass);
                            //checking if the type is not null
                            if (obClass == null)
                            {
                                //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                                //try getting the type from the assembly that call this one...
                                Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                obClass = MyAssembly.GetType(xob.GetAttribute(VALUE));
                                //Console.Out.WriteLine("NOW obClass: " + obClass);
                            }

                            map.Add(id, obClass);  //added Oct 2006
                            return obClass;
                        }
                        //if found in the array map
                        else
                        {
                            //Console.Out.WriteLine("Found in the Array Map: " + cSharpClass);
                            map.Add(id, cSharpClass);  //added Oct 2006
                            return cSharpClass;
                        }
                    }
                    //if found in the first map
                    else
                    {
                        //Console.Out.WriteLine("Found in the First Map: " + cSharpClass);
                        map.Add(id, cSharpClass);  //added Oct 2006
                        return cSharpClass;
                    }



                    //System.out.println("type: " + type + ", text: " + xob.getText());
                    //if it was a primitive class (i.e. double, boolean, etc.), then get it from the map
                    /*System.out.println("xob.getText()" + xob.getAttributeValue(VALUE));
                    Object primitiveClass = primitivesMap.get(xob.getAttributeValue(VALUE));
                    if (primitiveClass != null){
                         map.put(id, primitiveClass);  //added Oct 2006
                        return ((Class)primitiveClass);
                    }
                    //otherwise load the appropriate class and return it
                    //Object obClass = Class.forName(xob.getText());
                    Object obClass = Class.forName(xob.getAttributeValue(VALUE));
                    map.put(id, obClass);  //added Oct 2006
                    return obClass;*/
                }
                /******************************************/
                /*else if (type.equals(java.util.concurrent.atomic.AtomicLong.class)){
                    //System.out.println("it is atomic long...");
                    Class[] st = {long.class};
                    Constructor cons = type.getDeclaredConstructor(st);
                    // System.out.println("String Constructor: " + cons);
                    Object ob = makeObject(cons, new Object[]{new Long(xob.getText())}, id);
                    return ob;

                } */
                /********************************************/
                else
                {
                    //if it is a Character object - special case because Character has no constructor
                    //that takes a String. It only has a constructor that takes a char value
                    if (cSharpType.Equals(typeof(System.Char)))
                    {
                        //int decimalValue = getDecimalValue(xob.getText());
                        int decimalValue = getDecimalValue(xob.GetAttribute(VALUE));
                        Char charObject = (char)decimalValue;
                        //Console.Out.WriteLine("decimalvalue: " + decimalValue + ", charObject: " + charObject);
                        return charObject;
                        /*System.out.println("it is CHAR!!!");
                        st = new Class[]{char.class};
                        System.out.println("charText: " + charText + ", decimalValue: " + );*/
                    }
                    //for the rest of the Wrapper objects - they have constructors that take "String"
                    else if (cSharpType.Equals(typeof(System.SByte)))
                    {
                        return SByte.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int16)))
                    {
                        return Int16.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int32)))
                    {
                        return Int32.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int64)))
                    {
                        return Int64.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Single)))
                    {
                        return Single.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Double)))
                    {
                        return Double.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Boolean)))
                    {
                        return Boolean.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.String)))
                    {
                        return xob.GetAttribute(VALUE);
                    }
                    else
                    {


                        ///CHECK!!!!!!!!!!!!!!  April 2008
                        /*Class[] st = {String.class};
                         Constructor cons = type.getDeclaredConstructor(st);
                         //Object ob = makeObject(cons, new String[]{xob.getText()}, id);
                         Object ob = makeObject(cons, new String[]{xob.getAttributeValue(VALUE)}, id);
                         return ob;*/
                        return null;

                    }

                }
            }
            catch (Exception e)
            {

                //e.printStackTrace();
                Console.Out.WriteLine(e.Message);
                // System.out.println("While trying: " type );
                return null;
                // throw new RuntimeException(e);
            }

        }




        //////from here
        public Object readObject(XmlReader xob, string id)
        {
            // to read in an object we iterate over all the field elements
            // setting the corresponding field in the Object
            // first we construct an object of the correct class
            // this class may not have a public default constructor,
            // but will have a private default constructor - so we get
            // this back
            try
            {
                string typeName = xob.GetAttribute(TYPE);
                Type typeObject = Type.GetType(typeName);

                if (typeObject == null)
                {
                    typeObject = FindType(typeName);
                }

                ConstructorInfo cons = Util.forceDefaultConstructor(typeObject);
                Object ob = makeObject(cons, new Object[0], id);
                setFields(ob, xob);
                return ob;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private Type FindType(string typeName)
        {
            Type typeObject = null;

            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();

            // iterate each assembly to find our type
            Assembly foundType = null;
            try
            {
                var query = from assembly in asms.ToList()
                            where assembly.GetType(typeName) != null
                            select assembly;

                foundType = query.FirstOrDefault();
            }
            catch
            {
            }

            if (foundType != null)
            {
                typeObject = foundType.GetType(typeName);
            }

            return typeObject;
        }

        // this method not only makes the object, but also places
        // it in the HashMap of object references
        public Object makeObject(ConstructorInfo cons, Object[] args, string key)
        {
            Object value = cons.Invoke(args);
            map.Add(key, value);
            return value;
        }


        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="typeObject">The type object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FieldInfo getField(Type typeObject, String name)
        {
            if (typeObject == null)
            {
                return null;
            }
            try
            {
                FieldInfo[] fields = typeObject.GetFields(BindingFlags.Instance
                                                       | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo field = null;
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name.Equals(name))
                    {
                        field = fields[i];
                        break;
                    }
                }
                return field;
            }
            catch (Exception e)
            {
                return null;
            }
        }




        /// <summary>
        /// Sets the fields.
        /// </summary>
        /// <param name="ob">The ob.</param>
        /// <param name="xob">The xob.</param>
        public void setFields(Object ob, XmlReader xob)
        {
            Type typeObject = ob.GetType();

            XmlReader xobSubtree = xob.ReadSubtree();
            xobSubtree.Read();
            while (xobSubtree.Read())
            {
                if (xobSubtree.NodeType == XmlNodeType.Element)
                {
                    String name = xobSubtree.GetAttribute(NAME);
                    try
                    {
                        Type declaringType = typeObject;
                        Type propertyType = null;

                        MemberInfo info = getField(declaringType, name);

                        if (info == null)
                        {
                            // handle property set/get here
                            info = getProperty(declaringType, name);
                            if (info != null)
                            {
                                propertyType = (info as PropertyInfo).PropertyType;
                            }
                        }
                        else
                        {
                            propertyType = (info as FieldInfo).FieldType;
                        }

                        if (propertyType != null)
                        {
                            Object value = null;
                            if (Util.primitive(propertyType))
                            {
                                string attributeType = xobSubtree.GetAttribute(TYPE);
                                if (attributeType.Equals("char"))
                                {
                                    int decimalValue = getDecimalValue(xobSubtree.GetAttribute(VALUE));
                                    Char charObject = (char)decimalValue;
                                    value = charObject;
                                }
                                else if (attributeType.Equals("byte"))
                                {
                                    value = SByte.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("short"))
                                {
                                    value = Int16.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("int"))
                                {
                                    value = Int32.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("long"))
                                {
                                    value = Int64.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("float"))
                                {
                                    value = Single.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("double"))
                                {
                                    value = Double.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("boolean"))
                                {
                                    value = Boolean.Parse(xobSubtree.GetAttribute(VALUE));
                                }
                                else if (attributeType.Equals("string"))
                                {
                                    value = xobSubtree.GetAttribute(VALUE);
                                }
                                else if (attributeType.Equals("class"))
                                {
                                    value = Type.GetType(xobSubtree.GetAttribute(VALUE));
                                    if (value == null)
                                    {
                                        value = FindType(xobSubtree.GetAttribute(VALUE));
                                    }
                                }
                                else
                                {
                                    value = null;
                                }
                            }
                            else
                            {
                                xobSubtree.Read();
                                XmlReader xobSubSubTree = xobSubtree.ReadSubtree();
                                xobSubSubTree.Read();
                                value = read(xobSubSubTree);
                            }


                            if (info != null)
                            {
                                if (info is FieldInfo)
                                {
                                    (info as FieldInfo).SetValue(ob, value);
                                }
                                else if (info is PropertyInfo)
                                {
                                    (info as PropertyInfo).SetValue(ob, value, null);
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        private PropertyInfo getProperty(Type typeObject, string name)
        {
            if (typeObject == null)
            {
                return null;
            }
            try
            {
                PropertyInfo[] fields = typeObject.GetProperties(BindingFlags.Instance
                                                       | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
                PropertyInfo field = null;
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name.Equals(name))
                    {
                        field = fields[i];
                        break;
                    }
                }
                return field;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}

