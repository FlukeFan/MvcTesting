﻿using System.Linq;
using FluentAssertions;
using MvcTesting.Html;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Html
{
    [TestFixture]
    public class FormScraperTests
    {
        [Test]
        public void AllowsAccessToElement()
        {
            var html = "<form id='a_form'></form>";

            var form = new Response { Text = html }.Form<FormModel>();

            form.Element.Id.Should().Be("a_form");
        }

        [Test]
        public void AllowsMissingValues()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.GetText(m => m.Name).Should().BeNullOrEmpty();
        }

        [Test]
        public void TwoInputsWithSameName()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='value1' />
                    <input type='text' name='Name' value='value2' />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            var formValues = form.Get("Name");
            formValues.Length.Should().Be(1);
            formValues[0].ConfinedValues.Should().BeEquivalentTo("value1", "value2");
        }

        [Test]
        public void Text()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='form0' />
                    <input type='text' name='Name_readonly' readonly />
                    <input type='text' name='Name_disabled' disabled />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            var formValue = form.GetSingle("Name");

            formValue.Name.Should().Be("Name");
            formValue.Value.Should().Be("form0");
            formValue.Send.Should().BeTrue();

            form.GetSingle("Name_readonly").Readonly.Should().BeTrue("should be readonly");
            form.GetSingle("Name_disabled").Disabled.Should().BeTrue("should be disabled");
        }

        [Test]
        public void Checkbox()
        {
            var html = @"
                <form>
                    <input type='checkbox' name='cb_noValue_notChecked' />
                    <input type='checkbox' name='cb_noValue_checked' checked />
                    <input type='checkbox' name='cb_value_notChecked' value='123' />
                    <input type='checkbox' name='cb_value_checked' value='234' checked />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.GetSingle("cb_noValue_notChecked").Value.Should().Be("on");
            form.GetSingle("cb_noValue_notChecked").Send.Should().BeFalse();

            form.GetSingle("cb_noValue_checked").Value.Should().Be("on");
            form.GetSingle("cb_noValue_checked").Send.Should().BeTrue();

            form.GetSingle("cb_value_notChecked").Value.Should().Be("123");
            form.GetSingle("cb_value_notChecked").Send.Should().BeFalse();

            form.GetSingle("cb_value_checked").Value.Should().Be("234");
            form.GetSingle("cb_value_checked").Send.Should().BeTrue();
        }

        [Test]
        public void Radio()
        {
            var html = @"
                <form>
                    <input type='radio' name='r_noValue_checked' checked />
                    <input type='radio' name='r_value_notChecked' value='123' />
                    <input type='radio' name='r_value_notChecked' value='234' />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.GetSingle("r_noValue_checked").Value.Should().Be("on");
            form.GetSingle("r_noValue_checked").Send.Should().BeTrue();

            form.GetSingle("r_value_notChecked").ConfinedValues.Should().BeEquivalentTo("123", "234");
            form.GetSingle("r_value_notChecked").Send.Should().BeFalse();
        }

        [Test]
        public void Select()
        {
            var html = @"
                <form>
                    <select name='Selector'>
                        <option>notset</option>
                        <option value=''>empty</option>
                        <option value='v1' selected>value 1</option>
                        <option value='v2'>value 2</option>
                    </select>
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();
            var select = form.GetSingle("Selector");

            select.Value.Should().Be("v1");
            select.ConfinedValues.Should().BeEquivalentTo("notset", "", "v1", "v2");
            select.Texts.Should().BeEquivalentTo("notset", "empty", "value 1", "value 2");
        }

        [Test]
        public void TextArea()
        {
            var html = @"
                <form>
                    <textarea name='ta_disabled' disabled></textarea>
                    <textarea name='ta_readonly' readonly></textarea>
                    <textarea name='ta'>text area</textarea>
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.GetSingle("ta_disabled").Disabled.Should().BeTrue();
            form.GetSingle("ta_readonly").Readonly.Should().BeTrue();
            form.GetSingle("ta").Value.Should().Be("text area");
        }

        [Test]
        public void File()
        {
            var html = @"
                <form>
                    <input type='text' name='t' />
                    <input type='file' name='f' />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.FormValues.Count().Should().Be(1, "should have 1 text input");
            form.FileUploads.Count().Should().Be(1, "should have 1 file upload");

            form.GetFile("f").Content.Should().BeNull();
        }

        [Test]
        public void Submit()
        {
            var html = @"
                <form>
                    <input type='submit' />
                    <input type='submit' name='NoValue' />
                    <input type='submit' name='SubmitName' value='Submit Value' />
                    <input type='image'  name='ImageName'  value='Image Value'  />
                    <button>button</button>
                    <button type='submit'>buttonsubmit</button>
                    <button type='button'>notsubmit</button>
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.SubmitValues.Count().Should().Be(6);

            {
                var sb = form.SubmitValues.ElementAt(0);
                sb.Name.Should().Be("");
            }
            {
                var sb = form.SubmitValues.ElementAt(1);
                sb.Name.Should().Be("NoValue");
                sb.Value.Should().Be("Submit");
            }
            {
                var sb = form.SubmitValues.ElementAt(2);
                sb.Name.Should().Be("SubmitName");
                sb.Value.Should().Be("Submit Value");
            }
            {
                var sb = form.SubmitValues.ElementAt(3);
                sb.Name.Should().Be("ImageName");
                sb.Value.Should().Be("Image Value");
            }
            {
                var sb = form.SubmitValues.ElementAt(4);
                sb.Name.Should().Be("");
            }
            {
                var sb = form.SubmitValues.ElementAt(5);
                sb.Name.Should().Be("");
            }
        }
    }
}
