﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MvcTesting.Html;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Html
{
    [TestFixture]
    public class TypedFormTests
    {
        [Test]
        public void Get_NoMatchReturnsEmptyList()
        {
            var form = new TypedForm<FormModel>();

            var formValues = form.Get("DoesNotExist");

            formValues.Length.Should().Be(0);
        }

        [Test]
        public void GetSingle_NoMatch_ThrowsError()
        {
            var form = new TypedForm<FormModel>();

            var e = Assert.Throws<Exception>(() => form.GetSingle("DoesNotExist"));

            e.Message.Should().Be("Could not find entry 'DoesNotExist' in form values");
        }

        [Test]
        [Ignore("Until we decide what to do with multiple values with the same name")]
        public void GetSingle_MultipleMatch_ThrowsError()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='value1' />
                    <input type='text' name='Name' value='value2' />
                </form>
            ";

            var response = new Response { Text = html };
            var form = response.Form<FormModel>();

            var e = Assert.Throws<Exception>(() => form.GetSingle("Name"));

            e.Message.Should().Be("Found multiple form values for 'Name'");
        }

        [Test]
        public void Method_DefaultsToGet()
        {
            var html = @"<form />";
            var form = new Response { Text = html }.Form<FormModel>();

            form.Method.Should().Be("get");
        }

        [Test]
        public void Method_Post()
        {
            var html = @"<form method='post' />";
            var form = new Response { Text = html }.Form<FormModel>();

            form.Method.Should().Be("post");
        }

        [Test]
        public void Method_WhenNotGetOrPost_DefaultsToGet()
        {
            var html = @"<form method='put' />";
            var form = new Response { Text = html }.Form<FormModel>();

            form.Method.Should().Be("get");
        }

        [Test]
        public void Action()
        {
            var html = @"<form action='/test' />";
            var form = new Response { Text = html }.Form<FormModel>();

            form.Action.Should().Be("/test");
        }

        [Test]
        public void Action_DefaultsToEmpty()
        {
            var html = @"<form />";
            var form = new Response { Text = html }.Form<FormModel>();

            form.Action.Should().Be("");
        }

        [Test]
        public void AddFormValues()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='form0' />
                    <input type='text' name='Name2' value='form1' />
                    <input type='text' name='Name3' value='disabled inputs should not be set' disabled />
                    <input type='text' value='un-named values should not be set' />
                </form>
            ";

            var response = new Response { Text = html };

            var post = new Request("/tst", "POST");

            response.Form<FormModel>()
                .SetText(m => m.Name, "tst1")
                .SetText(m => m.Name2, "tst2")
                .AddFormValues(post);

            post.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Name", "tst1"),
                new NameValue("Name2", "tst2"),
            });
        }

        [Test]
        public void AddFormValuesUntyped()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='form0' />
                    <input type='file' name='Name3' />
                </form>
            ";

            var form = new Response { Text = html }
                .Form<FormModel>()
                .SetText("Name", "tst1")
                .SetFile("Name3", "tst2", Encoding.ASCII.GetBytes("tst2"));

            form.FormValues.Should().BeEquivalentTo(new FormValue[]
            {
                new FormValue("Name", "tst1"),
            });

            form.FileUploads.Should().BeEquivalentTo(new FileUpload[]
            {
                new FileUpload("Name3", "tst2", Encoding.ASCII.GetBytes("tst2")),
            });
        }

        [Test]
        public void AddFormValues_NoValues()
        {
            var get = new Request("/test?a=b");

            new TypedForm<object>()
                .AddFormValues(get);

            get.FormValues.Count().Should().Be(0);
            get.Query().Should().Be("");
        }

        [Test]
        public async Task Submit_SingleSubmit()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='hidden' name='Name1' value='Value1' />
                    <input type='submit' name='Submit1' value='Submit Value' />
                </form>
            ";

            var request = await FakeClient.Do(html, (form, client) =>
                form.Submit());

            request.Url.Should().Be("/test");
            request.Verb.Should().Be("GET");

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Name1", "Value1"),
                new NameValue("Submit1", "Submit Value"),
            });
        }

        [Test]
        public async Task Submit_SingleSubmitWithoutName()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='hidden' name='Name1' value='Value1' />
                    <input type='submit' value='Submit Value' />
                </form>
            ";

            var request = await FakeClient.Do(html, (form, client) =>
                form.Submit());

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Name1", "Value1"),
            });
        }

        [Test]
        public async Task Submit_NoButton()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='hidden' name='Name1' value='Value1' />
                </form>
            ";

            var request = await FakeClient.Do(html, (form, client) =>
                form.Submit(new SubmitValue()));

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Name1", "Value1"),
            });
        }

        [Test]
        public void Submit_WhenNoSingleSubmit_Throws()
        {
            var html = "<form/>";

            var e = Assert.ThrowsAsync<Exception>(() =>
                FakeClient.Do(html, (form, client) =>
                    form.Submit()));

            e.Message.Should().Be("Could not find single submit: count=0 ");
        }

        [Test]
        public void Submit_WhenMoreThanOneSubmit_Throws()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='submit' name='Submit1' value='Value1' />
                    <input type='submit' name='Submit2' value='Value2' />
                </form>
            ";

            var e = Assert.ThrowsAsync<Exception>(() =>
                FakeClient.Do(html, (form, client) =>
                    form.Submit()));

            e.Message.Should().Be("Could not find single submit: count=2 (Submit1=Value1), (Submit2=Value2)");
        }

        [Test]
        public async Task SubmitValue()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='submit' name='Submit' value='Value1' />
                    <input type='submit' name='Submit' value='Value2' />
                </form>
            ";

            var request = await FakeClient.Do(html, (form, client) =>
                form.SubmitValue("Value2"));

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Submit", "Value2"),
            });
        }

        [Test]
        public void SubmitValue_NotFound_Throws()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='submit' name='Submit' value='Value1' />
                    <input type='submit' name='Submit' value='Value2' />
                </form>
            ";

            Assert.ThrowsAsync<Exception>(() =>
                FakeClient.Do(html, (form, client) =>
                    form.SubmitValue("Value3")));
        }

        [Test]
        public void SubmitValue_FoundMultiple_Throws()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='submit' name='Submit1' value='Value1' />
                    <input type='submit' name='Submit2' value='Value1' />
                </form>
            ";

            Assert.ThrowsAsync<Exception>(() =>
                FakeClient.Do(html, (form, client) =>
                    form.SubmitValue("Value1")));
        }

        [Test]
        public async Task SubmitName()
        {
            var html = @"
                <form action='/test' method='get'>
                    <input type='submit' name='Submit1' value='Value1' />
                    <input type='submit' name='Submit2' value='Value2' />
                </form>
            ";

            var request = await FakeClient.Do(html, (form, client) =>
                form.SubmitName("Submit2"));

            request.FormValues.Should().BeEquivalentTo(new NameValue[]
            {
                new NameValue("Submit2", "Value2"),
            });
        }
    }
}
