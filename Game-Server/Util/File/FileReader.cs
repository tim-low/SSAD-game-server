﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Game_Server.Util
{
    public class FileReader : IEnumerable<FileReaderLine>, IDisposable
    {
        private readonly string _filePath;
        private readonly string _relativePath;
        private readonly StreamReader _streamReader;

        public FileReader(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File '" + filePath + "' not found.");

            _filePath = filePath;
            _relativePath = Path.GetDirectoryName(Path.GetFullPath(filePath));

            _streamReader = new StreamReader(filePath);
        }

        public int CurrentLine { get; protected set; }

        public void Dispose()
        {
            _streamReader.Close();
        }

        public IEnumerator<FileReaderLine> GetEnumerator()
        {
            string line;

            // Until EOF
            while ((line = _streamReader.ReadLine()) != null)
            {
                CurrentLine++;

                line = line.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Ignore very short or commented lines
                if (line.Length < 2 || line[0] == '!' || line[0] == ';' || line[0] == '#' || line.StartsWith("//") ||
                    line.StartsWith("--"))
                    continue;

                // Include files
                bool require = false, divert = false;
                if (line.StartsWith("include ") || (require = line.StartsWith("require ")) ||
                    (divert = line.StartsWith("divert ")))
                {
                    var fileName = line.Substring(line.IndexOf(' ')).Trim(' ', '"');
                    var includeFilePath = Path.Combine(!fileName.StartsWith("/") ? _relativePath : "",
                        fileName.TrimStart('/'));

                    // Prevent rekursive including
                    if (includeFilePath != _filePath)
                        if (File.Exists(includeFilePath))
                        {
                            using (var fr = new FileReader(includeFilePath))
                            {
                                foreach (var incLine in fr)
                                    yield return incLine;
                            }

                            // Stop reading current file if divert was successful
                            if (divert)
                                yield break;
                        }
                        else if (require)
                        {
                            throw new FileNotFoundException("Required file '" + includeFilePath + "' not found.");
                        }

                    continue;
                }

                yield return new FileReaderLine(line, _filePath);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

	public class FileReaderLine
	{
		/// <summary>
		///     New FileReaderLine.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="file"></param>
		public FileReaderLine(string line, string file)
		{
			Value = line;
			File = Path.GetFullPath(file);
		}

		/// <summary>
		///     Current line.
		/// </summary>
		public string Value { get; }

		/// <summary>
		///     Full path to the file the value was read from.
		/// </summary>
		public string File { get; }
	}
}