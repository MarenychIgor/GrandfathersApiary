"use strict";

function toogleItem(e, level) {
    var selector = `.js-child-${level}`;

    var parentNode = e.target.parentNode;
    var childElement = parentNode.querySelectorAll(selector);

    childElement.forEach(function (child) {
        child.classList.toggle("d-none");
    });
}