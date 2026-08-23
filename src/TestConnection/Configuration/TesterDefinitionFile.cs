using System;
using System.Collections.Generic;
using System.IO;

namespace TestConnection {
    internal static class TesterDefinitionFile {
        private const int ColumnCount = 5;

        public static List<TesterDefinition> Load(TextReader reader) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }

            List<TesterDefinition> definitions = new List<TesterDefinition>();
            string line;
            int lineNumber = 0;
            while ((line = reader.ReadLine()) != null) {
                lineNumber++;
                if (line.StartsWith("#", StringComparison.Ordinal)) {
                    continue;
                }

                try {
                    TesterDefinition definition = ParseLine(line);
                    string error = TesterDefinitionValidator.Validate(definition);
                    if (error != null) {
                        throw new FormatException(error);
                    }
                    definitions.Add(definition);
                }
                catch (Exception ex) {
                    throw new FormatException(lineNumber + "行: 読み込みファイルの形式が無効です。", ex);
                }
            }
            return definitions;
        }

        public static void Save(TextWriter writer, IEnumerable<TesterDefinition> definitions) {
            if (writer == null) {
                throw new ArgumentNullException("writer");
            }
            if (definitions == null) {
                throw new ArgumentNullException("definitions");
            }

            foreach (TesterDefinition definition in definitions) {
                writer.WriteLine(Serialize(definition));
            }
        }

        private static TesterDefinition ParseLine(string line) {
            if (line == null) {
                throw new ArgumentNullException("line");
            }

            string[] columns = line.Split(',');
            if (columns.Length < ColumnCount) {
                throw new FormatException("試験定義CSVの列数が不足しています。");
            }

            return new TesterDefinition(
                ParseEnum<TesterRole>(columns[0], " is Server/Client type error"),
                columns[1],
                columns[2],
                ParseEnum<ProtocolName>(columns[3], " is TCP/UDP/DNS/Ping type error"),
                Convert.ToInt32(columns[4]));
        }

        private static string Serialize(TesterDefinition definition) {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            return definition.Role.ToString() + "," +
                definition.LocalIpAddress + "," +
                definition.RemoteIpAddress + "," +
                definition.Protocol.ToString() + "," +
                definition.Port.ToString();
        }

        private static T ParseEnum<T>(string value, string errorSuffix) where T : struct {
            T parsed;
            if (Enum.TryParse(value, out parsed) && parsed.ToString() == value) {
                return parsed;
            }
            throw new FormatException(value + errorSuffix);
        }
    }
}
