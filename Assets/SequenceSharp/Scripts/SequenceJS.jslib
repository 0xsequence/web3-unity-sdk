mergeInto(LibraryManager.library, {
	Sequence_InitJSLibraries: function () {
		console.log("Initialized SequenceJS!");
		// var script = document.createElement("script");
		// script.src = "https://scriptURLhere";
		// document.head.appendChild(script);
	},

	Sequence_ExecuteJSInBrowserContext: function (jsString) {
		const func = Function(UTF8ToString(jsString));
		func();
	}
});