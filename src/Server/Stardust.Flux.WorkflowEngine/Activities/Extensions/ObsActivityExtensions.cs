using Elsa.Builders;
using System;
using System.Runtime.CompilerServices;

namespace Stardust.Flux.WorkflowEngine.Activities
{

    public static class ObsActivityExtensions
    {

        private static IActivityBuilder ObsConnect(this IBuilder builder, Action<ISetupActivity<ObsInitializeConnection>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.Then(setup, null, lineNumber, sourceFile);

        public static IActivityBuilder ObsConnect(this IBuilder builder, string host, uint port, string password, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
            builder.ObsConnect(setup => setup.UseHost(host).UsePort(port).UsePassword(password), lineNumber, sourceFile);

        public static IActivityBuilder ObsConnect(this IBuilder builder, string host, uint port, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
            builder.ObsConnect(setup => setup.UseHost(host).UsePort(port), lineNumber, sourceFile);

        public static IActivityBuilder ObsConnect(this IBuilder builder, string host, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
            builder.ObsConnect(setup => setup.UseHost(host).UsePort(4444), lineNumber, sourceFile);


        public static IActivityBuilder ObsConnect(this IBuilder builder, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
               builder.ObsConnect(setup => setup.UseHost("localhost").UsePort(4444), lineNumber, sourceFile);
        public static IActivityBuilder ObsStartStreaming(this IBuilder builder, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
         builder.Then<ObsStartStreaming>();

        public static IActivityBuilder ObsStopStreaming(this IBuilder builder, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
         builder.Then<ObsStopStreaming>();


        public static IActivityBuilder ObsStartRecording(this IBuilder builder, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.Then<ObsStartRecording>();

        public static IActivityBuilder ObsStopRecording(this IBuilder builder, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.Then<ObsStopRecording>();


        private static IActivityBuilder ObsSetCurrentScene(this IBuilder builder, Action<ISetupActivity<ObsSetCurrentScene>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
           builder.Then(setup, null, lineNumber, sourceFile);
        public static IActivityBuilder ObsSetCurrentScene(this IBuilder builder, string sceneName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
            builder.ObsSetCurrentScene(setup => setup.UseSceneName(sceneName), lineNumber, sourceFile);

        public static IActivityBuilder ObsSetCurrentScene(this IBuilder builder, uint sceneIndex, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string sourceFile = default) =>
         builder.ObsSetCurrentScene(setup => setup.UseSceneIndex(sceneIndex), lineNumber, sourceFile);

        private static IActivityBuilder ObsSetSourceVisibility(this IBuilder builder, Action<ISetupActivity<ObsSetSourceVisibility>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.Then(setup, null, lineNumber, sourceFile);

        public static IActivityBuilder ObsSetSourceVisibility(this IBuilder builder, string sourceName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.ObsSetSourceVisibility(setup => setup.UseSourceName(sourceName),  lineNumber, sourceFile);

        public static IActivityBuilder ObsSetSourceVisibility(this IBuilder builder, string sourceName,string sceneName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
       builder.ObsSetSourceVisibility(setup => setup.UseSourceName(sourceName).UseSceneName(sceneName), lineNumber, sourceFile);

        public static IActivityBuilder ObsSetSourceVisibility(this IBuilder builder, uint sourceIndex, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
      builder.ObsSetSourceVisibility(setup => setup.UseSourceIndex(sourceIndex), lineNumber, sourceFile);

        public static IActivityBuilder ObsSetSourceVisibility(this IBuilder builder, uint sourceIndex, string sceneName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
       builder.ObsSetSourceVisibility(setup => setup.UseSourceIndex(sourceIndex).UseSceneName(sceneName), lineNumber, sourceFile);


        private static IActivityBuilder ObsSetFFMPEGMediaSourceSettings(this IBuilder builder, Action<ISetupActivity<ObsSetFFMPEGMediaSourceSettings>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
         builder.Then(setup, null, lineNumber, sourceFile);

        public static IActivityBuilder ObsSetFFMPEGMediaSourceSettings(this IBuilder builder, string sourceName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.ObsSetFFMPEGMediaSourceSettings(setup => setup.UseSourceName(sourceName), lineNumber, sourceFile);

        private static IActivityBuilder ObsWaitForMediaEnded(this IBuilder builder, Action<ISetupActivity<ObsWaitForMediaEnded>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.Then(setup, null, lineNumber, sourceFile);

        public static IActivityBuilder ObsWaitForMediaEnded(this IBuilder builder, string sourceName, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
        builder.ObsWaitForMediaEnded(setup => setup.UseSourceName(sourceName), lineNumber, sourceFile);



    }

}
