"use strict";

var submitButton = document.querySelector(".js-search-button");
var searchInput = document.querySelector(".js-search-input");
var searchResultHolder = document.querySelector(".js-search-result");
var noResultSearchLabel = document.querySelector(".js-search-no-result");

submitButton.addEventListener("click", search);

function search(e) {
    e.preventDefault();
    sendRequest(searchInput.value);
}

function sendRequest(query) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            onSuccess(xhr.responseText);
        }
    };
    xhr.open("POST", "/api/feature/search/ajaxsearch");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send("{\"Query\":\"" + query + "\"}");
}

function onSuccess(data) {
    clearPreviousResults();

    var resultItems = JSON.parse(data);
    toogleEmptyResultLabel(resultItems.length == 0);

    resultItems.forEach(function (item) {
        addElement(item);
    });
}

function clearPreviousResults() {
    while (searchResultHolder.firstChild) {
        searchResultHolder.removeChild(searchResultHolder.firstChild);
    }
}

function addElement(item) {
    var resultHolder = document.createElement("div");
    var anchor = document.createElement("a");
    var span = document.createElement("span");

    anchor.setAttribute("href", item.Url);
    span.textContent = item.Name;

    anchor.appendChild(span);
    resultHolder.appendChild(anchor);
    searchResultHolder.appendChild(resultHolder);
}

function toogleEmptyResultLabel(isEmptyResult) {
    if (isEmptyResult) {
        noResultSearchLabel.classList.remove("d-none");
    } else {
        noResultSearchLabel.classList.add("d-none");
    }
}