using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMeta
{
    class CsvWriter : IDisposable
    {
        TextWriter m_writer;
        bool m_leaveOpen = false;

        public CsvWriter(TextWriter writer, bool leaveOpen = false)
        {
            m_writer = writer;
            m_leaveOpen = leaveOpen;
        }

        public CsvWriter(Stream stream, bool leaveOpen = false)
        {
            m_writer = new StreamWriter(stream, new UTF8Encoding(false), 4096, leaveOpen);
        }

        public CsvWriter(string filename, bool append=false)
        {
            m_writer = new StreamWriter(filename, append, new UTF8Encoding(false));
        }

        public void Write(IEnumerable<string> strings)
        {
            var e = strings.GetEnumerator();
            if (e.MoveNext())
            {
                for (; ; )
                {
                    if (e.Current != null)
                        m_writer.Write(CsvEncode(e.Current));
                    if (!e.MoveNext()) break;
                    m_writer.Write(',');
                }
            }
            m_writer.WriteLine();
        }

        public void Write(IEnumerable objects)
        {
            var e = objects.GetEnumerator();
            if (e.MoveNext())
            {
                for (; ; )
                {
                    if (e.Current != null)
                        m_writer.Write(CsvEncode(e.Current.ToString()));
                    if (!e.MoveNext()) break;
                    m_writer.Write(',');
                }
            }
            m_writer.WriteLine();
        }

        public void Write(params string[] strings)
        {
            Write((IEnumerable<string>)strings);
        }

        static char[] s_requiresEncoding = new char[] { ',', '\"', '\r', '\n' };

        public static string CsvEncode(string value)
        {
            if (value == null) return string.Empty;
            if (value.IndexOfAny(s_requiresEncoding) >= 0)
                return string.Concat("\"", value.Replace("\"", "\"\""), "\"");
            else
                return value;
        }

        public void Dispose()
        {
            if (m_writer != null)
            {
                if (m_leaveOpen)
                {
                    m_writer.Flush();
                }
                else
                {
                    m_writer.Dispose();
                }
                m_writer = null;
            }
        }
    }
}
