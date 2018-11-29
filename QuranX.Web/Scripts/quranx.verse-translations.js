function toggleTranslation(translatorCode, immediateHide) {
	var verseElem = $(".verse__translation[data-translator-code=\"" + translatorCode + "\"]");
	var hiddenCodeElem = $(".verse__translation-hidden-item[data-translator-code=\"" + translatorCode + "\"]");

	if (!immediateHide) {
		verseElem.toggle("collapse");
		hiddenCodeElem.toggle("collapse");
	} else {
		verseElem.hide();
		hiddenCodeElem.show();
	}
}
var quranXShowAllTranslations = (window.location.search || "").toLowerCase().indexOf("alltranslations=y") > -1;
// Buttons to show translations
$(".verse__translation-hidden-item")
	.on("click", function (ev) {
		const elem = $(ev.target);
		const translatorCode = elem.data("translator-code");
		window.localStorage.setItem("show-" + translatorCode, true);
		toggleTranslation(translatorCode);
	});
// Translations
$(".verse__translation>dt")
	.each(function (index, elem) {
		elem = $(elem.parentElement);
		const translatorCode = elem.data("translator-code");
		const useDefaultTranslationsKey = "useDefaultTranslations";
		const useDefaultTranslations = window.localStorage.getItem(useDefaultTranslationsKey);
		// If this is the first visit
		if (useDefaultTranslations !== "N") {
			// Hide the translation if not one of the defaults to show
			const defaultTranslations = ["AR", "Pickthall", "SahihIntl", "YusufAli"];
			for (var translationIndex = 0; translationIndex < defaultTranslations.length; translationIndex++) {
				const translatorCode = defaultTranslations[translationIndex];
				window.localStorage.setItem("show-" + translatorCode, true);
			}
			window.localStorage.setItem(useDefaultTranslationsKey, "N");
		}
		if (!quranXShowAllTranslations && !window.localStorage.getItem("show-" + translatorCode)) {
			toggleTranslation(translatorCode, true);
		};
	})
	.on("click", function (ev) {
		const elem = $(ev.target.parentElement);
		const translatorCode = elem.data("translator-code");
		window.localStorage.removeItem("show-" + translatorCode, true);
		toggleTranslation(translatorCode);
	});