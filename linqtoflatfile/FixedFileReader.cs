﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace LinqToFlatFile
{
    public class FixedFileReader<TEntity> : IFileReader<TEntity> where TEntity : new()
    {
        private char paddingNumber = '0';
        private char paddingChar = ' ';
        private TrimInputMode trimInput = TrimInputMode.Trim;
   
        public TrimInputMode TrimInput
        {
            get { return trimInput; }
            set { trimInput = value; }
        }

        public char PaddingChar
        {
            get { return paddingChar; }
            set { paddingChar = value; }
        }

        public char PaddingNumber
        {
            get { return paddingNumber; }
            set { paddingNumber = value; }
        }

        public IEnumerable<TEntity> ReadFile(Stream stream, bool headerRow)
        {
            var encoding = Encoding.GetEncoding(1252);
            //using (var reader = new StreamReader(stream, encoding))
            var reader = new StreamReader(stream, encoding);
            {
                string line;
                if (headerRow)
                {
                    //skip first row
                    reader.ReadLine();
                }
                while ((line = reader.ReadLine()) != null)
                {
                    var item = ReadLine(line);
                    yield return item;
                }
            }
        }

        public TEntity ReadLine(string line)
        {
            var entity = new TEntity();
            if (!String.IsNullOrEmpty(line))
            {
                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    foreach (
                        FixedPositionAttribute fixedFileAttribute in
                            property.GetCustomAttributes(typeof(FixedPositionAttribute), false))
                    {
                        if (null != fixedFileAttribute)
                        {
                            string substring = string.Empty;
                            if (fixedFileAttribute.StartPosition <= line.Length - 1)
                            {
                                substring = line.Substring(fixedFileAttribute.StartPosition,
                                                            Math.Min(
                                                                (fixedFileAttribute.EndPosition -
                                                                 fixedFileAttribute.StartPosition + 1),
                                                                line.Length - fixedFileAttribute.StartPosition));
                            }
                            switch (trimInput)
                            {
                                case TrimInputMode.Trim:
                                    substring = substring.Trim();
                                    break;
                                case TrimInputMode.TrimStart:
                                    substring = substring.TrimStart();
                                    break;
                                case TrimInputMode.TrimEnd:
                                    substring = substring.TrimEnd();
                                    break;
                            }

                            try
                            {
                                object theValue = null;
                                switch (property.PropertyType.ToString())
                                {
                                    case "System.Boolean":
                                        bool outbool;
                                        if (Boolean.TryParse(substring, out outbool))
                                        {
                                            outbool = false;
                                        }
                                        theValue = outbool;
                                        break;
                                    case "System.Int32":
                                        theValue = Int32.Parse(substring, CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Int16":
                                        theValue = Int16.Parse(substring, CultureInfo.InvariantCulture);
                                        break;
                                    case "System.DateTime":
                                        //<example>070228</example>yymmdd
                                        try
                                        {
                                            if (substring.Equals("000000"))
                                                theValue = DateTime.MinValue;
                                            else
                                            {
                                                int year = Int32.Parse(substring.Substring(0, 2),
                                                                       CultureInfo.InvariantCulture);
                                                if (year < 50)
                                                    year += 2000;
                                                else
                                                    year += 1900;
                                                int day = 1;
                                                int month = 1;
                                                if (substring.Length > 2)
                                                {
                                                    month = Int32.Parse(substring.Substring(2, 2),
                                                                        CultureInfo.InvariantCulture);
                                                    day = Int32.Parse(substring.Substring(4, 2),
                                                                      CultureInfo.InvariantCulture);
                                                }
                                                theValue = new DateTime(year, month, day);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new ArgumentOutOfRangeException(
                                                "Converting YYMMDD to DateTime failed for value:" + substring, ex);
                                        }
                                        break;
                                    case "System.Decimal":
                                        var MyCultureInfo = new CultureInfo("en-US");
                                        theValue = Decimal.Parse(substring, MyCultureInfo);
                                        if (!substring.Contains("."))
                                            theValue = (decimal)theValue / 100;
                                        break;
                                    case "System.String":
                                        theValue = substring;
                                        break;
                                    default:
                                        break;
                                }
                                property.SetValue(entity, theValue, null);
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentOutOfRangeException(
                                    "Converting from fixed file, field " + property.Name + " (" + property.PropertyType +
                                    ") failed for value: <" + substring + ">", ex);
                            }
                            break;
                        }
                    }
                }
            }
            return entity;
        }
    }
}