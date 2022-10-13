mergeInto(LibraryManager.library, {
	Sequence_ExecuteJSInBrowserContext: function (jsString) {
		eval(UTF8ToString(jsString));
	}
});