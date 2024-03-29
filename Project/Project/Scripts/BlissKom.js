﻿/** Denna filen innehåller ett antal script främst för att visa/gömma element och därigenom 
    exempelvis göra navigeringen inom ramen för en enskild sida mer responsiv.
    Hade detta inte varit ett asp.net- (och databas-)projekt hade jag lagt mer funktionalitet här eftersom
    det hade inneburit en mer responsiv användarupplevelse.
    */

"use strict";

var BlissKom = BlissKom || {};

BlissKom = {
    deleteMeaningConfirmed: false,
    deleteItemConfirmed: false,
    deleteMeaningIfConfirmed: function (message) {
        if (this.deleteMeaningConfirmed === true) {
            return true;
        }
        
        document.getElementById('Content_lblConfirm').innerText = message;
        this.showControl('Content_pnlConfirmBox');
        this.showControl('Content_btnOKConfirmDeleteMeaning');
        this.dimSimple(true);
        return false;
    },
    deleteItemIfConfirmed: function (message) {
        if (this.deleteItemConfirmed === true) {
            return true;
        }
        
        document.getElementById('Content_lblConfirm').innerText = message;
        this.showControl('Content_pnlConfirmBox');
        this.showControl('Content_btnOKConfirmDeleteItem');
        this.dimSimple(true);
        return false;
    },
    confirmDeleteMeaning: function () {
        this.deleteMeaningConfirmed = true;
        this.hideControl('Content_btnOKConfirmDeleteMeaning');
        this.hideControl('Content_pnlConfirmBox');
        this.undimSimple(true);
        document.getElementById('Content_btnDeleteMeaning').click();
    },
    confirmDeleteItem: function () {
        this.deleteItemConfirmed = true;
        this.hideControl('Content_btnOKConfirmDeleteItem');
        this.hideControl('Content_pnlConfirmBox');
        this.undimSimple(true);
        document.getElementById('Content_btnDeleteItem').click();
    },
    setDisplayBlock: function (id) {
        document.getElementById(id).style.display = "block";
    },
    removeDisplayNoneIfNotValid: function () {
        Page_ClientValidate();
        if (!Page_IsValid) {
            this.setDisplayBlock("Content_pnlErrorBox");
            this.dimSimple(true);
        }
    },
    enableControl: function (id) {
        document.getElementById(id).disabled = false;
    },
    disableControl: function (id) {
        document.getElementById(id).disabled = true;
    },
    hideControl: function (id) {
        document.getElementById(id).style.display = "none";
    },
    showControl: function (id) {
        document.getElementById(id).style.display = "block";
    },
    dimSimple: function (isSettings) {
        var pnl = document.getElementById("Content_pnlInnerTablet");
        var dimmer = document.createElement("div");
        if (isSettings === true) {
            dimmer.id = "dimmerSettings";
        } else {
            dimmer.id = "dimmer";
        }
        var unclickable = document.createElement("div");
        unclickable.id = "unclickable";
        pnl.appendChild(dimmer);
        pnl.appendChild(unclickable);
    },
    undimSimple: function (isSettings) {
        var dimmer;
        if (isSettings === true) {
            dimmer = document.getElementById("dimmerSettings");
        } else {
            dimmer = document.getElementById("dimmer");
        }
        if (dimmer) {
            dimmer.parentElement.removeChild(dimmer);
        }
        var unclickable = document.getElementById("unclickable");
        if (unclickable) {
            unclickable.parentElement.removeChild(unclickable);
        }
    },
    dim: function (pos) {
        this.dimSimple();
        var item = document.getElementById("Content_imbUnit" + pos);
        item.className = item.className.replace("item", "itemFull");
    },
    undim: function () {
        this.showCenterImage();
        this.undimSimple();
        var itemFull = document.getElementsByClassName("itemFull")[0];
        itemFull.className = itemFull.className.replace("itemFull", "item");
    },
    toggleNavButtons: function (ok, cancel, left, right, info) {
        var imgOK = document.getElementById("Content_imbOK");
        var imgCancel = document.getElementById("Content_imbCancel");
        var imgLeft = document.getElementById("Content_imbLeft");
        var imgRight = document.getElementById("Content_imbRight");
        var imgInfo = document.getElementById("Content_imbInfo");
        ok ? imgOK.style.display = "block" : imgOK.style.display = "none";
        cancel ? imgCancel.style.display = "block" : imgCancel.style.display = "none";
        left ? imgLeft.style.display = "block" : imgLeft.style.display = "none";
        right ? imgRight.style.display = "block" : imgRight.style.display = "none";
        info ? imgInfo.style.display = "block" : imgInfo.style.display = "none";
    },
    showCenterImage: function () {
        var currentLink = document.getElementsByClassName("itemFull")[0];
        var images = currentLink.querySelectorAll("[data-type]");
        for (var i = 0; i < images.length; i++) {
            images[i].style.display = "none";
            images[i].setAttribute("active", "false");
        }
        var newImage = currentLink.querySelector("[data-type = 'ParentWordItem']");
        newImage.style.display = "";
        newImage.setAttribute("active", "true");
    },
    showLeftImage: function () {
        var currentLink = document.getElementsByClassName("itemFull")[0];

        if (currentLink.querySelector("[data-type = 'ParentWordItem']").style.display === "") {
            var currentImage = currentLink.querySelector("[data-type = 'ParentWordItem']");
            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            var newImage = currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '1']");
            if (!currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '2']")) {
                document.getElementById("Content_imbLeft").style.display = "none";
            }
            newImage.style.display = "";
            document.getElementById("Content_imbRight").style.display = "block";
            newImage.setAttribute("active", "true");
        }
        else if (currentLink.querySelector("[data-type = 'ChildLeftWordItem'][active = 'true']")) {

            var currentImage = currentLink.querySelector("[data-type = 'ChildLeftWordItem'][active = 'true']");

            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            var newImage = currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '" + (parseInt(currentImage.getAttribute("data-pos")) + 1).toString() + "']");

            if (!currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) + 1).toString() + "']")) {
                document.getElementById("Content_imbLeft").style.display = "none";
            }
            if (!currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) - 1).toString() + "']")) {
                document.getElementById("Content_imbRight").style.display = "none";
            } else {
                document.getElementById("Content_imbRight").style.display = "block";
            }
            newImage.style.display = "";
            newImage.setAttribute("active", "true");
        } else if (currentLink.querySelector("[data-type = 'ChildRightWordItem'][active = 'true']")) {
            var currentImage = currentLink.querySelector("[data-type = 'ChildRightWordItem'][active = 'true']");

            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            if (currentImage.getAttribute("data-pos") === "1") {
                var newImage = currentLink.querySelector("[data-type = 'ParentWordItem']");

                if (!currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '1']")) {
                    document.getElementById("Content_imbLeft").style.display = "none";
                }
                document.getElementById("Content_imbRight").style.display = "block";
            } else {
                var newImage = currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(currentImage.getAttribute("data-pos")) - 1).toString() + "']");

                if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) - 1).toString() + "']")) {
                    document.getElementById("Content_imbRight").style.display = "none";
                }
                if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) + 1).toString() + "']")) {
                    document.getElementById("Content_imbLeft").style.display = "none";
                } else {
                    document.getElementById("Content_imbLeft").style.display = "block";
                }
            }
            newImage.style.display = "";
            newImage.setAttribute("active", "true");
        }
    },
    showRightImage: function () {
        var currentLink = document.getElementsByClassName("itemFull")[0];

        if (currentLink.querySelector("[data-type = 'ParentWordItem']").style.display === "") {
            var currentImage = currentLink.querySelector("[data-type = 'ParentWordItem']");
            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            var newImage = currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '1']");
            if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '2']")) {
                document.getElementById("Content_imbRight").style.display = "none";
            }
            newImage.style.display = "";
            document.getElementById("Content_imbLeft").style.display = "block";
            newImage.setAttribute("active", "true");
        }
        else if (currentLink.querySelector("[data-type = 'ChildRightWordItem'][active = 'true']")) {

            var currentImage = currentLink.querySelector("[data-type = 'ChildRightWordItem'][active = 'true']");

            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            var newImage = currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(currentImage.getAttribute("data-pos")) + 1).toString() + "']");

            if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) + 1).toString() + "']")) {
                document.getElementById("Content_imbRight").style.display = "none";
            }
            if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '" + (parseInt(newImage.getAttribute("data-pos")) - 1).toString() + "']")) {
                document.getElementById("Content_imbLeft").style.display = "none";
            } else {
                document.getElementById("Content_imbLeft").style.display = "block";
            }
            newImage.style.display = "";
            newImage.setAttribute("active", "true");
        } else if (currentLink.querySelector("[data-type = 'ChildLeftWordItem'][active = 'true']")) {
            var currentImage = currentLink.querySelector("[data-type = 'ChildLeftWordItem'][active = 'true']");

            currentImage.style.display = "none";
            currentImage.setAttribute("active", "false");
            if (currentImage.getAttribute("data-pos") === "1") {
                var newImage = currentLink.querySelector("[data-type = 'ParentWordItem']");

                if (!currentLink.querySelector("[data-type = 'ChildRightWordItem'][data-pos = '1']")) {
                    document.getElementById("Content_imbRight").style.display = "none";
                }
                document.getElementById("Content_imbLeft").style.display = "block";
            } else {
                var newImage = currentLink.querySelector("[data-type = 'ChildLeftWordItem'][data-pos = '" + (parseInt(currentImage.getAttribute("data-pos")) - 1).toString() + "']");

                document.getElementById("Content_imbLeft").style.display = "block";
            }
            newImage.style.display = "";
            newImage.setAttribute("active", "true");
        }
    }
};