﻿using System;
using System.Collections.Generic;
using Moq;
using Should;
using Xunit;

namespace Cassette.Scripts
{
    public class DebugScriptModuleHtmlRenderer_Tests
    {
        [Fact]
        public void GivenModuleWithTwoAssets_WhenRenderModule_ThenScriptsElementReturnedForEachAsset()
        {
            var module = new ScriptModule("~/test");
            module.AddAssets(new[] { Mock.Of<IAsset>(), Mock.Of<IAsset>() }, true);

            var urlGenerator = new Mock<IUrlGenerator>();
            var assetUrls = new Queue<string>(new[] { "asset1", "asset2" });
            urlGenerator.Setup(g => g.CreateAssetUrl(It.IsAny<IAsset>()))
                        .Returns(assetUrls.Dequeue);

            var renderer = new DebugScriptModuleHtmlRenderer(urlGenerator.Object);

            var html = renderer.Render(module).ToHtmlString();

            html.ShouldEqual(
                "<script src=\"asset1\" type=\"text/javascript\"></script>" + 
                Environment.NewLine +
                "<script src=\"asset2\" type=\"text/javascript\"></script>"
            );
        }

        [Fact]
        public void GivenModuleWithTransformedAsset_WhenRenderModule_ThenScriptElementHasCompiledUrl()
        {
            var module = new ScriptModule("~/test");
            var asset = new Mock<IAsset>();
            module.Assets.Add(asset.Object);
            asset.SetupGet(a => a.HasTransformers)
                 .Returns(true);

            var urlGenerator = new Mock<IUrlGenerator>();
            urlGenerator.Setup(g => g.CreateAssetCompileUrl(module, asset.Object))
                        .Returns("COMPILED-URL");

            var renderer = new DebugScriptModuleHtmlRenderer(urlGenerator.Object);

            var html = renderer.Render(module).ToHtmlString();
            html.ShouldEqual("<script src=\"COMPILED-URL\" type=\"text/javascript\"></script>");
        }
    }
}