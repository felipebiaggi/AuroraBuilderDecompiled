using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Builder.Presentation.Models.CharacterSheet;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Builder.Presentation.Models.CharacterSheet.Pages
{
    public abstract class PageGenerator : IDisposable
    {
        protected MemoryStream Stream { get; }

        protected Document Document { get; }

        protected PdfWriter Writer { get; }

        protected Dictionary<string, PdfReader> ResourceReaders { get; }

        public int PageFollowNumber { get; set; }

        public int PageWidth { get; protected set; }

        public int PageHeight { get; protected set; }

        public int PageMargin { get; protected set; }

        public int PageGutter { get; protected set; }

        public bool Flatten { get; set; }

        public List<string> PartialFlatteningNames { get; }

        protected PageGenerator(int pageFollowNumber = 1)
        {
            PageFollowNumber = pageFollowNumber;
            PageWidth = 612;
            PageHeight = 792;
            PageMargin = 26;
            PageGutter = 9;
            Flatten = false;
            PartialFlatteningNames = new List<string>();
            ResourceReaders = new Dictionary<string, PdfReader>();
            Stream = new MemoryStream();
            Document = new Document();
            Document.SetPageSize(new Rectangle(0f, 0f, PageWidth, PageHeight));
            Writer = PdfWriter.GetInstance(Document, Stream);
            Document.Open();
            Document.NewPage();
        }

        public MemoryStream GetStream()
        {
            Document.Close();
            Stream.Flush();
            return Stream;
        }

        public PdfReader AsReader()
        {
            return new PdfReader(GetStream().ToArray());
        }

        public void Save(string path, bool open = false)
        {
            Document.Close();
            Stream.Flush();
            PdfReader pdfReader = new PdfReader(Stream.ToArray());
            MemoryStream memoryStream = new MemoryStream();
            PdfStamper pdfStamper = new PdfStamper(pdfReader, memoryStream);
            if (Flatten)
            {
                pdfStamper.FormFlattening = true;
                foreach (string partialFlatteningName in PartialFlatteningNames)
                {
                    pdfStamper.PartialFormFlattening(partialFlatteningName);
                }
            }
            pdfStamper.Close();
            pdfReader.Close();
            File.WriteAllBytes(path, memoryStream.ToArray());
            memoryStream.Close();
            memoryStream.Dispose();
            if (open)
            {
                Process.Start(path);
            }
        }

        protected void PlacePage(PdfReader reader, Rectangle area)
        {
            PdfImportedPage importedPage = Writer.GetImportedPage(reader, 1);
            Writer.DirectContentUnder.AddTemplate(importedPage, area.Left, area.Bottom);
        }

        public virtual void StartNewPage()
        {
            Document.NewPage();
            PageFollowNumber++;
        }

        protected PdfReader GetResourceReader(string resourcePath)
        {
            if (!ResourceReaders.ContainsKey(resourcePath))
            {
                CharacterSheetResourcePage characterSheetResourcePage = new CharacterSheetResourcePage(resourcePath);
                ResourceReaders.Add(resourcePath, characterSheetResourcePage.CreateReader());
            }
            return ResourceReaders[resourcePath];
        }

        public virtual void Dispose()
        {
            Stream?.Dispose();
            Document?.Dispose();
            Writer?.Dispose();
            foreach (KeyValuePair<string, PdfReader> resourceReader in ResourceReaders)
            {
                resourceReader.Value?.Dispose();
            }
        }
    }
}
