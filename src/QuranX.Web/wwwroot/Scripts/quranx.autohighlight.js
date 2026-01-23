function getParameterByName(name) {
   const params = new URLSearchParams(window.location.search);
   return params.get(name) || "";
}

function isElementInViewport(el) {
   if (!el) return true;

   const rect = el.getBoundingClientRect();
   const viewHeight = window.innerHeight || document.documentElement.clientHeight;
   const viewWidth = window.innerWidth || document.documentElement.clientWidth;

   return (
     rect.top >= 0 &&
     rect.left >= 0 &&
     rect.bottom <= viewHeight &&
     rect.right <= viewWidth
   );
}

$(function () {
   const hl = getParameterByName("hl");

   if (hl && hl.trim().length > 0) {
     const terms = hl.split(",");

    $(".highlightable").highlight(terms);

    // jquery.highlight usually wraps matches in <span class="highlight">
    const firstHighlight = $(".highlight").first().get(0);

    if (firstHighlight && !isElementInViewport(firstHighlight)) {
     firstHighlight.scrollIntoView({
       behavior: "smooth",
       block: "center",
       inline: "nearest"
     });
    }
   }
});
