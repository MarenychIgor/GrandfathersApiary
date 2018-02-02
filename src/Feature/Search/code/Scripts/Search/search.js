"use strict";

var submitButton = document.querySelector(".js-search-button");
var searchInput = document.querySelector(".js-search-input");
var searchResultHolder = document.querySelector(".js-search-result");
var noResultSearchLabel = document.querySelector(".js-search-no-result");

if (submitButton) {
    submitButton.addEventListener("click", search);
}

var headerSearchInput = document.querySelector(".js-header-search-input");
var headerSearchResultHolder = document.querySelector(".js-header-search-result");
var headerNoResultSearchLabel = document.querySelector(".js-search-header-no-result");

headerSearchInput.addEventListener("keyup", headerSearch);

function headerSearch(e) {
    e.preventDefault();
    sendRequest(headerSearchInput.value, headerSearchResultHolder, headerNoResultSearchLabel);
}

function search(e) {
    e.preventDefault();
    sendRequest(searchInput.value, searchResultHolder, noResultSearchLabel);
}

function sendRequest(query, resultHolderElement, emptyResultLabel) {
    if (query == "") {
        clearPreviousResults(resultHolderElement);
        resultHolderElement.classList.add("d-none");
        return;
    }

    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            onSuccess(xhr.responseText, resultHolderElement, emptyResultLabel);
        }
    };
    xhr.open("POST", "/api/feature/search/ajaxsearch");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send("{\"Query\":\"" + query + "\"}");
}

function onSuccess(data, resultHolderElement, emptyResultLabel) {
    clearPreviousResults(resultHolderElement);
    clearSearchInput();

    var resultItems = JSON.parse(data);
    resultHolderElement.classList.remove("d-none");
    toogleEmptyResultLabel(resultItems.length == 0, emptyResultLabel);

    resultItems.forEach(function (item) {
        addElement(item, resultHolderElement);
    });
}

function clearPreviousResults(resultHolderElement) {
    while (resultHolderElement.firstChild) {
        resultHolderElement.removeChild(resultHolderElement.firstChild);
    }
}

function clearSearchInput() {
    if (searchInput) { // false for header search
        searchInput.value = '';
    }
}

function addElement(item, resultHolderElement) {
    var resultHolder = document.createElement("div");
    var anchor = document.createElement("a");
    var span = document.createElement("span");

    anchor.setAttribute("href", item.Url);
    span.textContent = item.Name;

    anchor.appendChild(span);
    resultHolder.appendChild(anchor);
    resultHolderElement.appendChild(resultHolder);
}

function toogleEmptyResultLabel(isEmptyResult, emptyResultLabel) {
    if (isEmptyResult) {
        emptyResultLabel.classList.remove("d-none");
    } else {
        emptyResultLabel.classList.add("d-none");
    }
}