function toggleTranslation(translatorCode, immediateHide) {
	var verseElem = $(`.verse__translation[data-translator-code="${translatorCode}"`);
	var hiddenCodeElem = $(`.verse__translation-hidden-item[data-translator-code="${translatorCode}"`);

	if (!immediateHide) {
		verseElem.toggle("collapse");
		hiddenCodeElem.toggle("collapse");
	} else {
		verseElem.hide();
		hiddenCodeElem.show();
	}
}
// Buttons to show translations
$(".verse__translation-hidden-item")
	.each(function (index, elem) {
		elem = $(elem);
		const translatorCode = elem.data("translator-code");
		if (window.localStorage.getItem(translatorCode)) {
			elem.show();
		} else {
			elem.hide();
		}
	})
	.on("click", function (ev) {
		const elem = $(ev.target);
		const translatorCode = elem.data("translator-code");
		window.localStorage.removeItem("hide-" + translatorCode);

		toggleTranslation(translatorCode);
	});
// Translations
$(".verse__translation>dt")
	.each(function (index, elem) {
		elem = $(elem.parentElement);
		const translatorCode = elem.data("translator-code");
		// If this is the first visit
		if (!window.localStorage.getItem("returnVisit")) {
			// Hide the translation if not one of the defaults to show
			if (["AR", "Pickthall", "SahihIntl", "YusufAli"].indexOf(translatorCode) < 0) {
				window.localStorage.setItem("hide-" + translatorCode, true);
			}
		}
		if (window.localStorage.getItem("hide-" + translatorCode)) {
			toggleTranslation(translatorCode, true);
		};
	})
	.on("click", function (ev) {
		const elem = $(ev.target.parentElement);
		const translatorCode = elem.data("translator-code");
		window.localStorage.setItem("hide-" + translatorCode, true);

		toggleTranslation(translatorCode);
	});
$(document).ready(function () {
	window.localStorage.setItem("returnVisit", true);
});
