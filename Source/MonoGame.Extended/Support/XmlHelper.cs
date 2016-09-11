using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
#if !USE_XMLDOCUMENT
using System.Xml.Linq;
#endif
using System.Xml.Schema;

namespace MonoGame.Extended.Support
{
    /// <summary>Helper routines for handling XML code</summary>
    public static class XmlHelper
    {
#if !NO_XMLSCHEMA

        #region class ValidationEventProcessor

        /// <summary>Handles any events occurring when an XML schema is loaded</summary>
        private class ValidationEventProcessor
        {

            /// <summary>
            ///   Callback for notifications being sent by the XmlSchema.Read() method
            /// </summary>
            /// <param name="sender">Not used</param>
            /// <param name="arguments">Contains the notification being sent</param>
            public void OnValidationEvent(object sender, ValidationEventArgs arguments)
            {
                // We're only interested in the first error, so if we already got
                // an exception on record, skip it!
                if (OccurredException != null)
                    return;
                
                // Only errors count as reasons to assume loading has failed
                if (arguments.Severity == XmlSeverityType.Error)
                {

                    // This code uses the ternary operator because I don't know how to
                    // stimlate a validation error that has no exception and couldn't achieve
                    // 100% test coverage otherwise. MSDN does not tell whether a validation
                    // error without an exception can even occur.
                    this.OccurredException = (arguments.Exception != null) ?
                      arguments.Exception :
                      new XmlSchemaValidationException("Unspecified schema validation error");

                }
            }

            /// <summary>Exception that has occurred during schema loading</summary>
            public Exception OccurredException;

        }

        #endregion // class ValidationEventProcessor

        /// <summary>Loads a schema from a file</summary>
        /// <param name="schemaPath">Path to the file containing the schema</param>
        /// <returns>The loaded schema</returns>
        public static XmlSchema LoadSchema(string schemaPath)
        {
            using (FileStream schemaStream = openFileForSharedReading(schemaPath))
            {
                return LoadSchema(schemaStream);
            }
        }

        /// <summary>Loads a schema from the provided stream</summary>
        /// <param name="schemaStream">Stream containing the schema that will be loaded</param>
        /// <returns>The loaded schema</returns>
        public static XmlSchema LoadSchema(Stream schemaStream)
        {
            return LoadSchema(new StreamReader(schemaStream));
        }

        /// <summary>Loads a schema from a text reader</summary>
        /// <param name="schemaReader">Text reader through which the schema can be read</param>
        /// <returns>The loaded schema</returns>
        public static XmlSchema LoadSchema(TextReader schemaReader)
        {
            ValidationEventProcessor eventProcessor = new ValidationEventProcessor();
            XmlSchema schema = XmlSchema.Read(schemaReader, eventProcessor.OnValidationEvent);
            if (eventProcessor.OccurredException != null)
            {
                throw eventProcessor.OccurredException;
            }
            return schema;
        }

        /// <summary>Attempts to load a schema from a file</summary>
        /// <param name="schemaPath">Path to the file containing the schema</param>
        /// <param name="schema">Output parameter that will receive the loaded schema</param>
        /// <returns>True if the schema was loaded successfully, otherwise false</returns>
        public static bool TryLoadSchema(string schemaPath, out XmlSchema schema)
        {
            FileStream schemaStream;
            if (!tryOpenFileForSharedReading(schemaPath, out schemaStream))
            {
                schema = null;
                return false;
            }

            using (schemaStream)
            {
                return TryLoadSchema(schemaStream, out schema);
            }
        }

        /// <summary>Attempts to load a schema from the provided stream</summary>
        /// <param name="schemaStream">Stream containing the schema that will be loaded</param>
        /// <param name="schema">Output parameter that will receive the loaded schema</param>
        /// <returns>True if the schema was loaded successfully, otherwise false</returns>
        public static bool TryLoadSchema(Stream schemaStream, out XmlSchema schema)
        {
            return TryLoadSchema(new StreamReader(schemaStream), out schema);
        }

        /// <summary>Attempts to load a schema from the provided text reader</summary>
        /// <param name="schemaReader">Reader from which the schema can be read</param>
        /// <param name="schema">Output parameter that will receive the loaded schema</param>
        /// <returns>True if the schema was loaded successfully, otherwise false</returns>
        public static bool TryLoadSchema(TextReader schemaReader, out XmlSchema schema)
        {
            try
            {
                ValidationEventProcessor eventProcessor = new ValidationEventProcessor();
                schema = XmlSchema.Read(schemaReader, eventProcessor.OnValidationEvent);
                if (eventProcessor.OccurredException == null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // Munch!
            }

            schema = null;
            return false;
        }

        /// <summary>Loads an XML document from a file</summary>
        /// <param name="schema">Schema to use for validating the XML document</param>
        /// <param name="documentPath">
        ///   Path to the file containing the XML document that will be loaded
        /// </param>
        /// <returns>The loaded XML document</returns>
        public static XDocument LoadDocument(XmlSchema schema, string documentPath)
        {
            using (FileStream documentStream = openFileForSharedReading(documentPath))
            {
                return LoadDocument(schema, documentStream);
            }
        }

        /// <summary>Loads an XML document from a stream</summary>
        /// <param name="schema">Schema to use for validating the XML document</param>
        /// <param name="documentStream">
        ///   Stream from which the XML document will be read
        /// </param>
        /// <returns>The loaded XML document</returns>
        public static XDocument LoadDocument(XmlSchema schema, Stream documentStream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schema);

            using (XmlReader reader = XmlReader.Create(documentStream, settings))
            {
                var document = XDocument.Load(reader, LoadOptions.None);

                // Create a schema set because the Validate() method only accepts
                // schemas in a schemaset
                var schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Perform the validation and report the first validation error
                // encountered to the caller
                var validationEventProcessor = new ValidationEventProcessor();
                document.Validate(schemas, validationEventProcessor.OnValidationEvent);
                if (validationEventProcessor.OccurredException != null)
                {
                    throw validationEventProcessor.OccurredException;
                }

                return document;
            }
        }

        /// <summary>Opens a file for shared reading</summary>
        /// <param name="path">Path to the file that will be opened</param>
        /// <returns>The opened file's stream</returns>
        private static FileStream openFileForSharedReading(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>Opens a file for shared reading</summary>
        /// <param name="path">Path to the file that will be opened</param>
        /// <param name="fileStream">
        ///   Output parameter that receives the opened file's stream
        /// </param>
        /// <returns>True if the file was opened successfully</returns>
        private static bool tryOpenFileForSharedReading(string path, out FileStream fileStream)
        {
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch (Exception)
            {
                // Munch!
            }

            fileStream = null;
            return false;
        }

#endif // !NO_XMLSCHEMA
    }
}
