﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcTesting.Http;

namespace MvcTesting.Html
{
    public static class FormHelper
    {
        public static async Task<TypedForm<T>> Form<T>(this Task<Response> response)
        {
            return (await response).Form<T>();
        }

        public static async Task<TypedForm<T>> Form<T>(this Task<Response> response, int index)
        {
            return (await response).Form<T>(index);
        }

        public static async Task<TypedForm<T>> Form<T>(this Task<Response> response, string cssSelector)
        {
            return (await response).Form<T>(cssSelector);
        }

        public static Func<ElementWrapper, FormScraper> NewScraper = ew => new FormScraper(ew);

        public static TypedForm<T> Scrape<T>(DocumentWrapper doc)
        {
            return Scrape<T>(doc, "form");
        }

        public static TypedForm<T> Scrape<T>(DocumentWrapper doc, int index)
        {
            var formElements = FindForms(doc, "form");

            if (index > formElements.Count - 1)
                throw new Exception(string.Format("Index '{0}' is too large for collection with '{1}' forms: {2}", index, formElements.Count, ElementWrapper.FormatTags(formElements)));

            return NewScraper(formElements[index]).Scrape<T>();
        }

        public static TypedForm<T> Scrape<T>(DocumentWrapper doc, string cssSelector)
        {
            var formElements = FindForms(doc, cssSelector);

            if (formElements.Count > 1)
                throw new Exception("Multiple form elements found in document: " + ElementWrapper.FormatTags(formElements));

            return NewScraper(formElements[0]).Scrape<T>();
        }

        private static List<ElementWrapper> FindForms(DocumentWrapper doc, string cssSelector)
        {
            var formElements = doc.FindAll(cssSelector);

            if (formElements.Count == 0)
                throw new Exception(string.Format("CSS selector '{0}' did not match any elements in the document", cssSelector));

            return formElements;
        }
    }
}
