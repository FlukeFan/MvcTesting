using System.Linq;

namespace MvcTesting.Html
{
    public class FormScraper
    {
        protected ElementWrapper    _element;

        public FormScraper(ElementWrapper element)
        {
            _element = element;
        }

        public virtual TypedForm<T> Scrape<T>()
        {
            var method = _element.AttributeOrEmpty("method");
            var action = _element.AttributeOrEmpty("action");
            var form = new TypedForm<T>(_element, method, action);
            AddInputs(form);
            return form;
        }

        protected virtual void AddInputs<T>(TypedForm<T> form)
        {
            var formInputs = _element.FindAll("input, select, textarea");

            var submits = formInputs.Where(i => IsSubmit(i));
            var inputs = formInputs.Where(i => !IsSubmit(i));

            foreach (var submit in submits)
                AddSubmit(form, submit);

            var formValues = FormValueScraper.FromElements(inputs);

            foreach (var formValue in formValues)
                form.AddFormValue(formValue);
        }

        protected virtual bool IsSubmit(ElementWrapper formInput)
        {
            var tagName = formInput.TagName.ToLower();
            var type = formInput.AttributeOrEmpty("type").ToLower();

            return tagName == "input"
                && (type == "submit" || type == "image");
        }

        protected virtual void AddSubmit<T>(TypedForm<T> form, ElementWrapper inputSubmit)
        {
            var submitValue = SubmitValue.FromElement(inputSubmit);
            form.AddSubmitValue(submitValue);
        }
    }
}
