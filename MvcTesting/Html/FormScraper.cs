using System.Linq;

namespace MvcTesting.Html
{
    public class FormScraper
    {
        protected ElementWrapper        _element;
        protected ISimulatedHttpClient  _client;

        public FormScraper(ElementWrapper element, ISimulatedHttpClient client = null)
        {
            _element = element;
            _client = client;
        }

        public virtual TypedForm<T> Scrape<T>()
        {
            var method = _element.AttributeOrEmpty("method");
            var action = _element.AttributeOrEmpty("action");
            var form = new TypedForm<T>(_client, _element, method, action);
            AddInputs(form);
            return form;
        }

        protected virtual void AddInputs<T>(TypedForm<T> form)
        {
            var formInputs = _element.FindAll("input, select, textarea, button");

            var submits = formInputs.Where(i => IsSubmit(i));
            var inputs = formInputs.Where(i => IsInput(i));
            var fileUploads = formInputs.Where(i => IsFileUpload(i));

            foreach (var submit in submits)
                AddSubmit(form, submit);

            var formValues = FormValueScraper.FromElements(inputs);

            foreach (var formValue in formValues)
                form.AddFormValue(formValue);

            foreach (var fileUpload in fileUploads)
                form.AddFile(new FileUpload(fileUpload.Attribute("name"), null, null));
        }

        protected virtual bool IsSubmit(ElementWrapper formInput)
        {
            var tagName = formInput.TagName.ToLower();
            var type = formInput.AttributeOrEmpty("type")?.ToLower();

            var isInputSubmit = tagName == "input" && (type == "submit" || (type == "image"));
            var isButtonSubmit = tagName == "button" && (string.IsNullOrWhiteSpace(type) || type == "submit");

            return isInputSubmit || isButtonSubmit;
        }

        protected virtual bool IsInput(ElementWrapper formInput)
        {
            if (IsSubmit(formInput))
                return false;

            var tagName = formInput.TagName.ToLower();
            var type = formInput.AttributeOrEmpty("type")?.ToLower();
            return tagName != "button" && type != "file";
        }

        protected virtual bool IsFileUpload(ElementWrapper formInput)
        {
            var type = formInput.AttributeOrEmpty("type")?.ToLower();
            return type == "file";
        }

        protected virtual void AddSubmit<T>(TypedForm<T> form, ElementWrapper inputSubmit)
        {
            var submitValue = SubmitValue.FromElement(inputSubmit);
            form.AddSubmitValue(submitValue);
        }
    }
}
