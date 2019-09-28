var skuforsearch;
var skuAndProductSearch = 0;
var tagSearch = 0;
var product_id;
var product_text;
var myself;
var productSubCategoriesAction = [];
var attribute_id;
var data_CategoriesAjax;
$(document).ready(function () {
    if (data_CategoriesAjax == null || data_CategoriesAjax.length == 0) {
        GetCategoriesAjax();
    }

    window.AllTagsList = [];
    $('.tagsData').each(function () {
        var ProductTagValue = $(this).attr('data-attributevalue');
        var ProductTagId = $(this).attr('data-attributeid');
        if (typeof ProductTagValue !== typeof undefined && ProductTagValue !== false && ProductTagValue != "undefined") {
            var obj = { attribute_id: ProductTagId, value: ProductTagValue, sku: "" }
            window.AllTagsList.push(obj);
        }
    });

    console.log(window.AllTagsList);

    var this_product_id = $('#product_name').attr('data-id');
    skuforsearch = [];
    var tagsforsearch = [];
    $.ajax({
        type: 'GET',
        data: { 'product_id': this_product_id },
        url: '/product/PairWithProductsAutocomplete/' + this_product_id,
        dataType: 'JSON',
        success: function (result) {
            skuforsearch = result;
            var brandProductSKU = [];
            var brandProducts = [];
            $(skuforsearch).each(function (i, val) {
                brandProductSKU[i] = { value: val.vendor + ' - ' + val.title + ' - ' + val.value, product_id: val.product_id, sku: val.value };
                brandProducts[val.product_id] = { value: val.vendor + ' - ' + val.title + ' - ' + val.value };
            })

            $('.productAutoCompleteList .bootstrap-tagsinput > input').autocompleteCustom({
                lookup: brandProductSKU
            });

            $('#tagSearchRow .bootstrap-tagsinput > input').autocompleteCustom({
                lookup: window.AllTagsList
            });

            if (brandProductSKU.length > 0) {
                inputcheckedvalue(brandProducts);
            }
        }
    });

    $('#StyleSubType').multiselect({
        nonSelectedText: 'Select sub type',
        enableFiltering: true,
        templates: {
            li: '<li><a href="javascript:void(0);"><label class="pl-2"></label></a></li>',
            filter: '<li class="multiselect-item filter"><div class="input-group m-0 mb-1"><input class="form-control multiselect-search" type="text"></div></li>',
            filterClearBtn: '<div class="input-group-append"><button class="btn btn btn-primary multiselect-clear-filter" type="button"><i class="fa fa-times"></i></button></div>'
        },
        selectedClass: 'bg-light',
        onInitialized: function (select, container) {
            // hide checkboxes
            container.find('input[type=checkbox]').addClass('d-none');
        },
        onChange: function (option, checked) {
            var actionType = checked ? "insert" : "delete";
            productSubCategoriesAction = productSubCategoriesAction.filter(function (category) {
                return category.product_category_id != $(option).val();
            });
            var categoryAction = { category_id: $(option).val(), action: actionType };
            productSubCategoriesAction.push(categoryAction)

            if (productSubCategoriesAction.length > 0) {

                $('#StyleSubType').removeClass('errorFeild');
                $('#StyleSubType').parents('.form-group').find('span.errorMsg').remove()
            }

        }
    });

    if (selectedProductCategories != null || selectedProductCategories != "") {
        var selectProductCategories = JSON.parse(selectedProductCategories);
        var categoryIds = [];
        for (var i = 0; i < selectProductCategories.length; i++) {
            var Category = selectProductCategories[i];
            categoryIds.push(Category.category_id);
        }
        $('#StyleSubType').multiselect('select', categoryIds);
    }

    function inputcheckedvalue(productsdata) {
        $(".checkedvalue").each(function (i, val) {

            var checked = '';
            var mcCbxCheck = $(this);
            if (mcCbxCheck.is(':checked')) {
                checked = 'checked'
            }
            tagsforsearch[i] = { value: val.value, data: checked }

            checked = '';
            if (val.value.indexOf("pairwith") > -1) {
                $(".extratags span#" + val.value).text(productsdata[val.value.replace("pairwith", "")].value).show();
            }
            if (val.value.indexOf("matchingset") > -1) {
                $(".extratags span#" + val.value).text(productsdata[val.value.replace("matchingset", "")].value).show();
            }
            if (val.value.indexOf("same-color-as-") > -1) {
                $(".extratags span#" + val.value).text(productsdata[val.value.replace("same-color-as-", "")].value).show();
            }

        });

        $('#autocompleteCustom').autocompleteCustom({
            lookup: tagsforsearch
        });
    }

    $(document).on("click", ".SKU_for_tags", function () {
        product_id = $(this).attr('product_id');
        product_text = $(this).text();
        myself.add(product_text);

        $('#PairWithRow .bootstrap-tagsinput input').val('');

    })

    $(document).on("click", ".SKU_for_tags_color", function () {
        product_id = $(this).attr('product_id');
        product_text = $(this).text();
        myself.add(product_text);
        $('#OtherColorsRow .bootstrap-tagsinput input').val('');
    })


    $(document).on("click", ".SKU_for_tags_match", function () {
        product_id = "";
        attribute_id = $(this).attr('attribute_id');
        product_text = $(this).text();
        myself.add(product_text);
        $('#tagSearchRow .bootstrap-tagsinput input').val('');
        if ($("#tagsDataId-" + attribute_id).prop("checked") == false) {
            $(".autocompleteCustom-suggestions").css("display", "none");
            //$("#tagsDataId-" + attribute_id).prop("checked", true);
            $("#tagsDataId-" + attribute_id).click();
        }
    })


    $(document).on("keypress click", "#PairWithRow .bootstrap-tagsinput input", function () {

        skuAndProductSearch = 1;
        tagSearch = 0;
        $('.autocompleteCustom-suggestions').css('width', '62%');


    })

    $(document).on("keypress click", "#OtherColorsRow .bootstrap-tagsinput input", function () {
        skuAndProductSearch = 2;
        $('.autocompleteCustom-suggestions').css('width', '62%');

    })

    $(document).on("keypress click", "#tagSearchRow .bootstrap-tagsinput input", function () {
        skuAndProductSearch = 3;
        $('.autocompleteCustom-suggestions').css('width', '62%');

    })

    $(document).on("keypress click", "#autocompleteCustom", function () {
        skuAndProductSearch = 0;
        tagSearch = 1;
        $('.autocompleteCustom-suggestions').css('width', '16%');

    })

    $(document).on("click", ".dropdown-for-type li", function () {
        $('.product-type-value').html(this.innerHTML + '<span class="caret"></span>');
        $('#producttypedrop').val(this.innerHTML);
        $('.updateTagsToShopify').css('display', 'block');
        $('#savedsuccessfully').css('display', 'none');
    })

    $(document).on("change", ".checkedvalue", function () {
        $('.updateTagsToShopify').css('display', 'block');
        $('#savedsuccessfully').css('display', 'none');
        var checkeditem = '';
        if ($(this).is(":checked")) {
            checkeditem = this.value;
            $('input[type=checkbox][value=' + this.value + ']').prop('checked', true);
        }
        else {
            $('input[type=checkbox][value=' + this.value + ']').prop('checked', false);

        }
        $("#errormsgformultipletypes").css('display', 'none')
        if ($(this).hasClass('selectonlyone')) {
            checkecCoount = 0;
            $(".selectonlyone").each(function () {

                if ($(this).is(':checked')) {
                    checkecCoount++;
                }

            });
            if (checkecCoount > 1) {
                $('input[type=checkbox][value=' + checkeditem + ']').prop('checked', false);
                if ($('.updateTagsToShopify').is(':visible')) {
                    $('.updateTagsToShopify').css('display', 'none');
                    $("#errormsgformultipletypes").css('display', 'block')
                }
                else {
                    $('.updateTagsToShopify').css('display', 'block');
                }

            }

        }

    });






});


function GetCategoriesAjax() {

    $.ajax({
        url: '/categoryoption/GetCategories',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {

        data_CategoriesAjax = data;




        $('#StyleSubType option').remove();
        $('#StyleSubType').multiselect('rebuild');
        var DDL_ProductType = $('#DDL_ProductType').val();

        if (DDL_ProductType != 0) {

            var subCategories = data_CategoriesAjax.filter(function (category) {
                return category.category_parent_id == DDL_ProductType;
            });

            if (subCategories.length) {
                for (var i = 0; i < subCategories.length; i++) {
                    $('#StyleSubType').append("<option value='" + subCategories[i].category_id + "'>" + subCategories[i].category_name + "</option >")
                }
                $('#StyleSubType').multiselect('rebuild');

            }

            if (selectedProductCategories != null || selectedProductCategories != "") {
                var selectProductCategories = JSON.parse(selectedProductCategories);
                var categoryIds = [];
                for (var i = 0; i < selectProductCategories.length; i++) {
                    var Category = selectProductCategories[i];
                    categoryIds.push(Category.category_id);
                }
                $('#StyleSubType').multiselect('select', categoryIds);
            }
        }
    });

}












/**
 *  Ajax autocompleteCustom for jQuery, version 1.2.7
 *  (c) 2013 Tomas Kirda
 *
 *  Ajax autocompleteCustom for jQuery is freely distributable under the terms of an MIT-style license.
 *  For details, see the web site: http://www.devbridge.com/projects/autocompleteCustom/jquery/
 *
 */
(function (e) {
    "function" === typeof define && define.amd ? define(["jquery"], e) : e(jQuery)
})(function (e) {
    function g(a, b) {
        var c = function () { },
            c = {
                autoSelectFirst: !1,
                appendTo: "body",
                serviceUrl: null,
                lookup: null,
                onSelect: null,
                width: "auto",
                minChars: 1,
                maxHeight: 300,
                deferRequestBy: 0,
                params: {},
                formatResult: g.formatResult,
                delimiter: null,
                zIndex: 9999,
                type: "GET",
                noCache: !1,
                onSearchStart: c,
                onSearchComplete: c,
                containerClass: "autocompleteCustom-suggestions",
                tabDisabled: !1,
                dataType: "text",
                lookupFilter: function (a, b, c) {
                    return -1 !==
                        a.value.toLowerCase().indexOf(c)
                },
                paramName: "query",
                transformResult: function (a) {
                    return "string" === typeof a ? e.parseJSON(a) : a
                }
            };
        this.element = a;
        this.el = e(a);
        this.suggestions = [];
        this.badQueries = [];
        this.selectedIndex = -1;
        this.currentValue = this.element.value;
        this.intervalId = 0;
        this.cachedResponse = [];
        this.onChange = this.onChangeInterval = null;
        this.isLocal = this.ignoreValueChange = !1;
        this.suggestionsContainer = null;
        this.options = e.extend({}, c, b);
        this.classes = {
            selected: "autocompleteCustom-selected",
            suggestion: "autocompleteCustom-suggestion"
        };
        this.initialize();
        this.setOptions(b)
    }
    var h = {
        extend: function (a, b) {
            return e.extend(a, b)
        },
        createNode: function (a) {
            var b = document.createElement("div");
            b.innerHTML = a;
            return b.firstChild
        }
    };
    g.utils = h;
    e.autocompleteCustom = g;
    g.formatResult = function (a, b) {
        var c = "(" + b.replace(RegExp("(\\/|\\.|\\*|\\+|\\?|\\||\\(|\\)|\\[|\\]|\\{|\\}|\\\\)", "g"), "\\$1") + ")";
        return a.value.replace(RegExp(c, "gi"), "<strong>$1</strong>")
    };
    g.prototype = {
        killerFn: null,
        initialize: function () {
            var a = this,
                b = "." + a.classes.suggestion,
                c = a.classes.selected,
                d = a.options,
                f;
            a.element.setAttribute("autocompleteCustom", "off");
            a.killerFn = function (b) {
                0 === e(b.target).closest("." + a.options.containerClass).length && (a.killSuggestions(), a.disableKillerFn())
            };
            if (!d.width || "auto" === d.width) d.width = a.el.outerWidth();
            a.suggestionsContainer = g.utils.createNode('<div class="' + d.containerClass + '" style="position: absolute; display: none;"></div>');
            f = e(a.suggestionsContainer);
            f.appendTo(d.appendTo).width(d.width);
            f.on("mouseover.autocompleteCustom", b, function () {
                a.activate(e(this).data("index"))
            });
            f.on("mouseout.autocompleteCustom", function () {
                a.selectedIndex = -1;
                f.children("." + c).removeClass(c)
            });
            f.on("click.autocompleteCustom", b, function () {
                //a.select(e(this).data("index"), !1)
            });
            a.fixPosition();
            if (window.opera) a.el.on("keypress.autocompleteCustom", function (b) {
                a.onKeyPress(b)
            });
            else a.el.on("keydown.autocompleteCustom", function (b) {
                a.onKeyPress(b)
            });
            a.el.on("keyup.autocompleteCustom", function (b) {
                a.onKeyUp(b)
            });
            a.el.on("blur.autocompleteCustom", function () {
                a.onBlur()
            });
            a.el.on("focus.autocompleteCustom", function () {
                a.fixPosition()
            })
        },
        onBlur: function () {
            this.enableKillerFn()
        },
        setOptions: function (a) {
            var b = this.options;
            h.extend(b, a);
            if (this.isLocal = e.isArray(b.lookup)) b.lookup = this.verifySuggestionsFormat(b.lookup);
            e(this.suggestionsContainer).css({
                "max-height": b.maxHeight + "px",
                width: b.width + "px",
                "z-index": b.zIndex
            })
        },
        clearCache: function () {
            this.cachedResponse = [];
            this.badQueries = []
        },
        clear: function () {
            this.clearCache();
            this.currentValue = null;
            this.suggestions = []
        },
        disable: function () {
            this.disabled = !0
        },
        enable: function () {
            this.disabled = !1
        },
        fixPosition: function () {
            var a;
            "body" === this.options.appendTo &&
                (a = this.el.offset(), e(this.suggestionsContainer).css({
                    top: a.top + this.el.outerHeight() + "px",
                    left: a.left + "px"
                }))
        },
        enableKillerFn: function () {
            e(document).on("click.autocompleteCustom", this.killerFn)
        },
        disableKillerFn: function () {
            e(document).off("click.autocompleteCustom", this.killerFn)
        },
        killSuggestions: function () {
            var a = this;
            a.stopKillSuggestions();
            a.intervalId = window.setInterval(function () {
                a.hide();
                a.stopKillSuggestions()
            }, 300)
        },
        stopKillSuggestions: function () {
            window.clearInterval(this.intervalId)
        },
        onKeyPress: function (a) {
            $('.checkedvalue').parent('td').parent('tr').css('background-color', 'white');
            if (!this.disabled &&
                !this.visible && 40 === a.keyCode && this.currentValue) this.suggest();
            else if (!this.disabled && this.visible) {
                switch (a.keyCode) {
                    case 27:
                        this.el.val(this.currentValue);
                        this.hide();
                        break;
                    case 9:
                    case 13:
                        if (-1 === this.selectedIndex) {
                            this.hide();
                            return
                        }
                        this.select(this.selectedIndex, 13 === a.keyCode);
                        if (9 === a.keyCode && !1 === this.options.tabDisabled) return;
                        break;
                    case 38:
                        this.moveUp();
                        break;
                    case 40:
                        this.moveDown();
                        break;
                    default:
                        return
                }
                a.stopImmediatePropagation();
                a.preventDefault()
            }
        },
        onKeyUp: function (a) {
            var b = this;
            if (!b.disabled) {
                switch (a.keyCode) {
                    case 38:
                    case 40:
                        return
                }
                clearInterval(b.onChangeInterval);
                if (b.currentValue !== b.el.val())
                    if (0 < b.options.deferRequestBy) b.onChangeInterval = setInterval(function () {
                        b.onValueChange()
                    }, b.options.deferRequestBy);
                    else b.onValueChange()
            }
        },
        onValueChange: function () {
            var a;
            clearInterval(this.onChangeInterval);
            this.currentValue = this.element.value;
            a = this.getQuery(this.currentValue);
            this.selectedIndex = -1;
            this.ignoreValueChange ? this.ignoreValueChange = !1 : a.length < this.options.minChars ?
                this.hide() : this.getSuggestions(a)
        },
        getQuery: function (a) {
            var b = this.options.delimiter;
            if (!b) return e.trim(a);
            a = a.split(b);
            return e.trim(a[a.length - 1])
        },
        getSuggestionsLocal: function (a) {
            var b = a.toLowerCase(),
                c = this.options.lookupFilter;
            return {
                suggestions: e.grep(this.options.lookup, function (d) {
                    return c(d, a, b)
                })
            }
        },
        getSuggestions: function (a) {
            var b, c = this,
                d = c.options,
                f = d.serviceUrl;
            (b = c.isLocal ? c.getSuggestionsLocal(a) : c.cachedResponse[a]) && e.isArray(b.suggestions) ? (c.suggestions = b.suggestions, c.suggest()) :
                c.isBadQuery(a) || (d.params[d.paramName] = a, !1 !== d.onSearchStart.call(c.element, d.params) && (e.isFunction(d.serviceUrl) && (f = d.serviceUrl.call(c.element, a)), e.ajax({
                    url: f,
                    data: d.ignoreParams ? null : d.params,
                    type: d.type,
                    dataType: d.dataType
                }).done(function (b) {
                    c.processResponse(b, a);
                    d.onSearchComplete.call(c.element, a)
                })))
        },
        isBadQuery: function (a) {
            for (var b = this.badQueries, c = b.length; c--;)
                if (0 === a.indexOf(b[c])) return !0;
            return !1
        },
        hide: function () {
            this.visible = !1;
            this.selectedIndex = -1;
            e(this.suggestionsContainer).hide()
        },
        suggest: function () {
            //console.log('aaa',this);
            const startTime = performance.now();
            if (0 === this.suggestions.length) this.hide();
            else {
                var a = this.options.formatResult,
                    b = this.getQuery(this.currentValue),
                    c = this.classes.suggestion,
                    d = this.classes.selected,
                    f = e(this.suggestionsContainer),
                    g = "";


                e.each(this.suggestions, function (d, e) {
                    if (skuAndProductSearch == 1) {

                        g += ' <div  class="' + c + ' SKU_for_tags" product_id="' + e.product_id + '" data-index="' + d + '">' + a(e, b) + "</div>"
                    }
                    else if (skuAndProductSearch == 2) {
                        g += ' <div  class="' + c + ' SKU_for_tags_color" product_id="' + e.product_id + '" data-index="' + d + '">' + a(e, b) + "</div>"
                    } else if (skuAndProductSearch == 3) {
                        g += ' <div  class="' + c + ' SKU_for_tags_match" attribute_id="' + e.attribute_id + '" data-index="' + d + '">' + a(e, b) + "</div>"
                    }
                    else {
                        pairwith = e.value;
                        if (!(pairwith.indexOf('PairWith') > -1)) {

                            $($('input[type=checkbox][value=' + e.value + ']').parent('td').parent('tr')[0]).css('background-color', 'rgb(255, 251, 120)');
                            g += ' <div class="' + c + '" data-index="' + d + '"><input type="checkbox" class="checkedvalue" style="margin-right: 8px;  " value="' + e.value + '" ' + e.data + ' >' + a(e, b) + "</div>"
                        }
                        else {
                            g += ' <div style="display: none;  " class="' + c + '" data-index="' + d + '"><input type="checkbox" class="checkedvalue" style="margin-right: 8px;  " value="' + e.value + '" ' + e.data + ' >' + a(e, b) + "</div>"
                        }

                    }

                });
                f.html(g).show();
                this.visible = !0;
                this.options.autoSelectFirst && (this.selectedIndex = 0, f.children().first().addClass(d))
            }
            const duration = performance.now() - startTime;
            //console.log(`someMethodIThinkMightBeSlow took ${duration}ms`);
        },
        verifySuggestionsFormat: function (a) {
            return a.length && "string" ===
                typeof a[0] ? e.map(a, function (a) {
                    return {
                        value: a,
                        data: null
                    }
                }) : a
        },
        processResponse: function (a, b) {
            var c = this.options,
                d = c.transformResult(a, b);
            d.suggestions = this.verifySuggestionsFormat(d.suggestions);
            c.noCache || (this.cachedResponse[d[c.paramName]] = d, 0 === d.suggestions.length && this.badQueries.push(d[c.paramName]));
            b === this.getQuery(this.currentValue) && (this.suggestions = d.suggestions, this.suggest())
        },
        activate: function (a) {
            var b = this.classes.selected,
                c = e(this.suggestionsContainer),
                d = c.children();
            c.children("." +
                b).removeClass(b);
            this.selectedIndex = a;
            return -1 !== this.selectedIndex && d.length > this.selectedIndex ? (a = d.get(this.selectedIndex), e(a).addClass(b), a) : null
        },
        select: function (a, b) {
            var c = this.suggestions[a];
            c && (this.el.val(c), this.ignoreValueChange = b, this.hide(), this.onSelect(a))
        },
        moveUp: function () {
            -1 !== this.selectedIndex && (0 === this.selectedIndex ? (e(this.suggestionsContainer).children().first().removeClass(this.classes.selected), this.selectedIndex = -1, this.el.val(this.currentValue)) : this.adjustScroll(this.selectedIndex -
                1))
        },
        moveDown: function () {
            // this.selectedIndex !== this.suggestions.length - 1 && this.adjustScroll(this.selectedIndex + 1)
        },
        adjustScroll: function (a) {
            var b = this.activate(a),
                c, d;
            b && (b = b.offsetTop, c = e(this.suggestionsContainer).scrollTop(), d = c + this.options.maxHeight - 25, b < c ? e(this.suggestionsContainer).scrollTop(b) : b > d && e(this.suggestionsContainer).scrollTop(b - this.options.maxHeight + 25), this.el.val(this.getValue(this.suggestions[a].value)))
        },
        onSelect: function (a) {
            var b = this.options.onSelect;
            a = this.suggestions[a];
            this.el.val(this.getValue(a.value));
            e.isFunction(b) && b.call(this.element, a)
        },
        getValue: function (a) {
            var b = this.options.delimiter,
                c;
            if (!b) return a;
            c = this.currentValue;
            b = c.split(b);
            return 1 === b.length ? a : c.substr(0, c.length - b[b.length - 1].length) + a
        },
        dispose: function () {
            this.el.off(".autocompleteCustom").removeData("autocompleteCustom");
            this.disableKillerFn();
            e(this.suggestionsContainer).remove()
        }
    };
    e.fn.autocompleteCustom = function (a, b) {
        return 0 === arguments.length ? this.first().data("autocompleteCustom") : this.each(function () {
            var c =
                e(this),
                d = c.data("autocompleteCustom");
            if ("string" === typeof a) {
                if (d && "function" === typeof d[a]) d[a](b)
            } else d && d.dispose && d.dispose(), d = new g(this, a), c.data("autocompleteCustom", d)
        })
    }
});



(function ($) {
    "use strict";



    var defaultOptions = {
        tagClass: function (item) {
            return 'label label-info';
        },
        focusClass: 'focus',
        itemValue: function (item) {
            return item ? item.toString() : item;
        },
        itemText: function (item) {

            return this.itemValue(item);
        },
        itemTitle: function (item) {
            return null;
        },
        freeInput: true,
        addOnBlur: false,
        maxTags: undefined,
        maxChars: undefined,
        confirmKeys: [],
        delimiter: ',',
        delimiterRegex: null,
        cancelConfirmKeysOnEmpty: false,
        onTagExists: function (item, $tag) {
            $tag.hide().fadeIn();
        },
        trimValue: false,
        allowDuplicates: false,
        triggerChange: true
    };

    /**
     * Constructor function
     */
    function TagsInput(element, options) {

        this.isInit = true;
        this.itemsArray = [];

        this.$element = $(element);
        this.$element.hide();

        this.isSelect = (element.tagName === 'SELECT');
        this.multiple = (this.isSelect && element.hasAttribute('multiple'));
        this.objectItems = options && options.itemValue;
        this.placeholderText = element.hasAttribute('placeholder') ? this.$element.attr('placeholder') : '';
        this.inputSize = Math.max(1, this.placeholderText.length);

        this.$container = $('<div class="bootstrap-tagsinput"></div>');
        this.$input = $('<input type="text" placeholder="' + this.placeholderText + '"/>').appendTo(this.$container);

        this.$element.before(this.$container);

        this.build(options);
        this.isInit = false;
    }

    TagsInput.prototype = {
        constructor: TagsInput,

        /**
         * Adds the given item as a new tag. Pass true to dontPushVal to prevent
         * updating the elements val()
         */
        add: function (item, dontPushVal, options) {


            var self = this;

            if (self.options.maxTags && self.itemsArray.length >= self.options.maxTags)
                return;

            // Ignore falsey values, except false
            if (item !== false && !item)
                return;

            // Trim value
            if (typeof item === "string" && self.options.trimValue) {
                item = $.trim(item);
            }
            // Throw an error when trying to add an object while the itemValue option was not set
            /* if (typeof item === "object" && !self.objectItems)
               throw("Can't add objects when itemValue option is not set");*/

            // Ignore strings only containg whitespace
            if (item.toString().match(/^\s*$/))
                return;

            // If SELECT but not multiple, remove current tag
            if (self.isSelect && !self.multiple && self.itemsArray.length > 0)
                self.remove(self.itemsArray[0]);

            if (typeof item === "string" && this.$element[0].tagName === 'INPUT') {


                var delimiter = (self.options.delimiterRegex) ? self.options.delimiterRegex : self.options.delimiter;
                var items = item.split(delimiter);
                if (items.length > 1) {
                    for (var i = 0; i < items.length; i++) {
                        this.add(items[i], true);

                    }

                    if (!dontPushVal)
                        self.pushVal(self.options.triggerChange);
                    return;
                }
            }

            var itemValue = self.options.itemValue(item),
                itemText = self.options.itemText(item),
                tagClass = self.options.tagClass(item),
                itemTitle = self.options.itemTitle(item);

            // Ignore items allready added
            var existing = $.grep(self.itemsArray, function (item) { return self.options.itemValue(item) === itemValue; })[0];
            if (existing && !self.options.allowDuplicates) {
                // Invoke onTagExists
                if (self.options.onTagExists) {
                    var $existingTag = $(".tag", self.$container).filter(function () { return $(this).data("item") === existing; });
                    self.options.onTagExists(item, $existingTag);
                }
                return;
            }

            // if length greater than limit
            if (self.items().toString().length + item.length + 1 > self.options.maxInputLength)
                return;

            // raise beforeItemAdd arg
            var beforeItemAddEvent = $.Event('beforeItemAdd', { item: item, cancel: false, options: options });
            self.$element.trigger(beforeItemAddEvent);
            if (beforeItemAddEvent.cancel)
                return;

            // register item in internal array and map
            self.itemsArray.push(item);

            // add a tag element
            if (typeof product_id !== typeof undefined && product_id !== false && product_id != "undefined" && product_id != "") {
                var $tag = $('<span class="tag pairwithallproductsid ' + htmlEncode(tagClass) + (itemTitle !== null ? ('" title="' + itemTitle) : '') + '" product_id="' + product_id + '">' + htmlEncode(itemText) + '<span data-role="remove"></span></span>');

                $tag.data('item', item);
                self.findInputWrapper().before($tag);
                $tag.after(' ');
            }
            else {
                var $tag = "";/*  $('<span class="tag selectedProductTags ' + htmlEncode(tagClass) + (itemTitle !== null ? ('" title="' + itemTitle) : '') + '" attribute_id="' + attribute_id + '">' + htmlEncode(itemText) + '<span data-role="remove"></span></span>'); */
            }



            // Check to see if the tag exists in its raw or uri-encoded form
            var optionExists = (
                $('option[value="' + encodeURIComponent(itemValue) + '"]', self.$element).length ||
                $('option[value="' + htmlEncode(itemValue) + '"]', self.$element).length
            );

            // add <option /> if item represents a value not present in one of the <select />'s options
            if (self.isSelect && !optionExists) {

                var $option = $('<option selected>' + htmlEncode(itemText) + '</option>');
                $option.data('item', item);
                $option.attr('value', itemValue);
                product_id
                self.$element.append($option);
            }

            if (!dontPushVal)
                self.pushVal(self.options.triggerChange);

            // Add class when reached maxTags
            if (self.options.maxTags === self.itemsArray.length || self.items().toString().length === self.options.maxInputLength)
                self.$container.addClass('bootstrap-tagsinput-max');

            // If using typeahead, once the tag has been added, clear the typeahead value so it does not stick around in the input.
            if ($('.typeahead, .twitter-typeahead', self.$container).length) {
                self.$input.typeahead('val', '');
            }

            if (this.isInit) {
                self.$element.trigger($.Event('itemAddedOnInit', { item: item, options: options }));
            } else {
                self.$element.trigger($.Event('itemAdded', { item: item, options: options }));
            }


        },

        /**
         * Removes the given item. Pass true to dontPushVal to prevent updating the
         * elements val()
         */
        remove: function (item, dontPushVal, options) {
            var self = this;

            if (self.objectItems) {
                if (typeof item === "object")
                    item = $.grep(self.itemsArray, function (other) { return self.options.itemValue(other) == self.options.itemValue(item); });
                else
                    item = $.grep(self.itemsArray, function (other) { return self.options.itemValue(other) == item; });

                item = item[item.length - 1];
            }

            if (item) {
                var beforeItemRemoveEvent = $.Event('beforeItemRemove', { item: item, cancel: false, options: options });
                self.$element.trigger(beforeItemRemoveEvent);
                if (beforeItemRemoveEvent.cancel)
                    return;

                $('.tag', self.$container).filter(function () { return $(this).data('item') === item; }).remove();
                $('option', self.$element).filter(function () { return $(this).data('item') === item; }).remove();
                if ($.inArray(item, self.itemsArray) !== -1)
                    self.itemsArray.splice($.inArray(item, self.itemsArray), 1);
            }

            if (!dontPushVal)
                self.pushVal(self.options.triggerChange);

            // Remove class when reached maxTags
            if (self.options.maxTags > self.itemsArray.length)
                self.$container.removeClass('bootstrap-tagsinput-max');

            self.$element.trigger($.Event('itemRemoved', { item: item, options: options }));
        },

        /**
         * Removes all items
         */
        removeAll: function () {
            var self = this;

            $('.tag', self.$container).remove();
            $('option', self.$element).remove();

            while (self.itemsArray.length > 0)
                self.itemsArray.pop();

            self.pushVal(self.options.triggerChange);
        },

        /**
         * Refreshes the tags so they match the text/value of their corresponding
         * item.
         */
        refresh: function () {
            var self = this;
            $('.tag', self.$container).each(function () {
                var $tag = $(this),
                    item = $tag.data('item'),
                    itemValue = self.options.itemValue(item),
                    itemText = self.options.itemText(item),
                    tagClass = self.options.tagClass(item);

                // Update tag's class and inner text
                $tag.attr('class', null);
                $tag.addClass('tag ' + htmlEncode(tagClass));
                $tag.contents().filter(function () {
                    return this.nodeType == 3;
                })[0].nodeValue = htmlEncode(itemText);

                if (self.isSelect) {
                    var option = $('option', self.$element).filter(function () { return $(this).data('item') === item; });
                    option.attr('value', itemValue);
                }
            });
        },

        /**
         * Returns the items added as tags
         */
        items: function () {
            return this.itemsArray;
        },

        /**
         * Assembly value by retrieving the value of each item, and set it on the
         * element.
         */
        pushVal: function () {
            var self = this,
                val = $.map(self.items(), function (item) {
                    return self.options.itemValue(item).toString();
                });

            self.$element.val(val, true);

            if (self.options.triggerChange)
                self.$element.trigger('change');
        },

        /**
         * Initializes the tags input behaviour on the element
         */
        build: function (options) {
            var self = this;

            self.options = $.extend({}, defaultOptions, options);
            // When itemValue is set, freeInput should always be false
            if (self.objectItems)
                self.options.freeInput = false;

            makeOptionItemFunction(self.options, 'itemValue');
            makeOptionItemFunction(self.options, 'itemText');
            makeOptionFunction(self.options, 'tagClass');

            // Typeahead Bootstrap version 2.3.2
            if (self.options.typeahead) {
                var typeahead = self.options.typeahead || {};

                makeOptionFunction(typeahead, 'source');

                self.$input.typeahead($.extend({}, typeahead, {
                    source: function (query, process) {
                        function processItems(items) {
                            var texts = [];

                            for (var i = 0; i < items.length; i++) {
                                var text = self.options.itemText(items[i]);
                                map[text] = items[i];
                                texts.push(text);
                            }
                            process(texts);
                        }

                        this.map = {};
                        var map = this.map,
                            data = typeahead.source(query);

                        if ($.isFunction(data.success)) {
                            // support for Angular callbacks
                            data.success(processItems);
                        } else if ($.isFunction(data.then)) {
                            // support for Angular promises
                            data.then(processItems);
                        } else {
                            // support for functions and jquery promises
                            $.when(data)
                                .then(processItems);
                        }
                    },
                    updater: function (text) {
                        self.add(this.map[text]);
                        return this.map[text];
                    },
                    matcher: function (text) {
                        return (text.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1);
                    },
                    sorter: function (texts) {
                        return texts.sort();
                    },
                    highlighter: function (text) {
                        var regex = new RegExp('(' + this.query + ')', 'gi');
                        return text.replace(regex, "<strong>$1</strong>");
                    }
                }));
            }

            // typeahead.js
            if (self.options.typeaheadjs) {
                // Determine if main configurations were passed or simply a dataset
                var typeaheadjs = self.options.typeaheadjs;
                if (!$.isArray(typeaheadjs)) {
                    typeaheadjs = [null, typeaheadjs];
                }

                $.fn.typeahead.apply(self.$input, typeaheadjs).on('typeahead:selected', $.proxy(function (obj, datum, name) {
                    var index = 0;
                    typeaheadjs.some(function (dataset, _index) {
                        if (dataset.name === name) {
                            index = _index;
                            return true;
                        }
                        return false;
                    });

                    // @TODO Dep: https://github.com/corejavascript/typeahead.js/issues/89
                    if (typeaheadjs[index].valueKey) {
                        self.add(datum[typeaheadjs[index].valueKey]);
                    } else {
                        self.add(datum);
                    }

                    self.$input.typeahead('val', '');
                }, self));
            }

            self.$container.on('click', $.proxy(function (event) {
                if (!self.$element.attr('disabled')) {
                    self.$input.removeAttr('disabled');
                }
                self.$input.focus();
            }, self));

            if (self.options.addOnBlur && self.options.freeInput) {
                self.$input.on('focusout', $.proxy(function (event) {
                    // HACK: only process on focusout when no typeahead opened, to
                    //       avoid adding the typeahead text as tag
                    if ($('.typeahead, .twitter-typeahead', self.$container).length === 0) {
                        self.add(self.$input.val());
                        self.$input.val('');
                    }
                }, self));
            }

            // Toggle the 'focus' css class on the container when it has focus
            self.$container.on({
                focusin: function () {
                    self.$container.addClass(self.options.focusClass);
                },
                focusout: function () {
                    self.$container.removeClass(self.options.focusClass);
                },
            });

            self.$container.on('keydown', 'input', $.proxy(function (event) {
                var $input = $(event.target),
                    $inputWrapper = self.findInputWrapper();

                if (self.$element.attr('disabled')) {
                    self.$input.attr('disabled', 'disabled');
                    return;
                }

                switch (event.which) {
                    // BACKSPACE
                    case 8:
                        if (doGetCaretPosition($input[0]) === 0) {
                            var prev = $inputWrapper.prev();
                            if (prev.length) {
                                self.remove(prev.data('item'));
                            }
                        }
                        break;

                    // DELETE
                    case 46:
                        if (doGetCaretPosition($input[0]) === 0) {
                            var next = $inputWrapper.next();
                            if (next.length) {
                                self.remove(next.data('item'));
                            }
                        }
                        break;

                    // LEFT ARROW
                    case 37:
                        // Try to move the input before the previous tag
                        var $prevTag = $inputWrapper.prev();
                        if ($input.val().length === 0 && $prevTag[0]) {
                            $prevTag.before($inputWrapper);
                            $input.focus();
                        }
                        break;
                    // RIGHT ARROW
                    case 39:
                        // Try to move the input after the next tag
                        var $nextTag = $inputWrapper.next();
                        if ($input.val().length === 0 && $nextTag[0]) {
                            $nextTag.after($inputWrapper);
                            $input.focus();
                        }
                        break;
                    default:
                    // ignore
                }

                // Reset internal input's size
                var textLength = $input.val().length,
                    wordSpace = Math.ceil(textLength / 5),
                    size = textLength + wordSpace + 1;
                $input.attr('size', Math.max(this.inputSize, $input.val().length));
            }, self));

            self.$container.on('keypress', 'input', $.proxy(function (event) {
                var $input = $(event.target);
                if (self.$element.attr('disabled')) {
                    self.$input.attr('disabled', 'disabled');
                    return;
                }

                var text = $input.val(),
                    maxLengthReached = self.options.maxChars && text.length >= self.options.maxChars;
                myself = self;
                if (self.options.freeInput && (keyCombinationInList(event, self.options.confirmKeys) || maxLengthReached)) {

                    // Only attempt to add a tag if there is data in the field
                    if (text.length !== 0) {

                        self.add(maxLengthReached ? text.substr(0, self.options.maxChars) : text);
                        $input.val('');
                    }

                    // If the field is empty, let the event triggered fire as usual
                    if (self.options.cancelConfirmKeysOnEmpty === false) {
                        event.preventDefault();
                    }
                }

                // Reset internal input's size
                var textLength = $input.val().length,
                    wordSpace = Math.ceil(textLength / 5),
                    size = textLength + wordSpace + 1;
                $input.attr('size', Math.max(this.inputSize, $input.val().length));
            }, self));

            // Remove icon clicked
            self.$container.on('click', '[data-role=remove]', $.proxy(function (event) {
                if (self.$element.attr('disabled')) {
                    return;
                }
                self.remove($(event.target).closest('.tag').data('item'));
            }, self));

            // Only add existing value as tags when using strings as tags
            if (self.options.itemValue === defaultOptions.itemValue) {
                if (self.$element[0].tagName === 'INPUT') {
                    self.add(self.$element.val());
                } else {
                    $('option', self.$element).each(function () {
                        self.add($(this).attr('value'), true);
                    });
                }
            }
        },

        /**
         * Removes all tagsinput behaviour and unregsiter all event handlers
         */
        destroy: function () {
            var self = this;

            // Unbind events
            self.$container.off('keypress', 'input');
            self.$container.off('click', '[role=remove]');

            self.$container.remove();
            self.$element.removeData('tagsinput');
            self.$element.show();
        },

        /**
         * Sets focus on the tagsinput
         */
        focus: function () {
            this.$input.focus();
        },

        /**
         * Returns the internal input element
         */
        input: function () {
            return this.$input;
        },

        /**
         * Returns the element which is wrapped around the internal input. This
         * is normally the $container, but typeahead.js moves the $input element.
         */
        findInputWrapper: function () {
            var elt = this.$input[0],
                container = this.$container[0];
            while (elt && elt.parentNode !== container)
                elt = elt.parentNode;

            return $(elt);
        }
    };

    /**
     * Register JQuery plugin
     */
    $.fn.tagsinput = function (arg1, arg2, arg3) {
        var results = [];

        this.each(function () {
            var tagsinput = $(this).data('tagsinput');
            // Initialize a new tags input
            if (!tagsinput) {
                tagsinput = new TagsInput(this, arg1);
                $(this).data('tagsinput', tagsinput);
                results.push(tagsinput);

                if (this.tagName === 'SELECT') {
                    $('option', $(this)).attr('selected', 'selected');
                }

                // Init tags from $(this).val()
                $(this).val($(this).val());
            } else if (!arg1 && !arg2) {
                // tagsinput already exists
                // no function, trying to init
                results.push(tagsinput);
            } else if (tagsinput[arg1] !== undefined) {
                // Invoke function on existing tags input
                if (tagsinput[arg1].length === 3 && arg3 !== undefined) {
                    var retVal = tagsinput[arg1](arg2, null, arg3);
                } else {
                    var retVal = tagsinput[arg1](arg2);
                }
                if (retVal !== undefined)
                    results.push(retVal);
            }
        });

        if (typeof arg1 == 'string') {
            // Return the results from the invoked function calls
            return results.length > 1 ? results : results[0];
        } else {
            return results;
        }
    };

    $.fn.tagsinput.Constructor = TagsInput;

    /**
     * Most options support both a string or number as well as a function as
     * option value. This function makes sure that the option with the given
     * key in the given options is wrapped in a function
     */
    function makeOptionItemFunction(options, key) {

        if (typeof options[key] !== 'function') {
            var propertyName = options[key];
            options[key] = function (item) { return item[propertyName]; };
        }
    }
    function makeOptionFunction(options, key) {
        if (typeof options[key] !== 'function') {
            var value = options[key];
            options[key] = function () { return value; };
        }
    }
    /**
     * HtmlEncodes the given value
     */
    var htmlEncodeContainer = $('<div />');
    function htmlEncode(value) {
        if (value) {
            return htmlEncodeContainer.text(value).html();
        } else {
            return '';
        }
    }

    /**
     * Returns the position of the caret in the given input field
     * http://flightschool.acylt.com/devnotes/caret-position-woes/
     */
    function doGetCaretPosition(oField) {
        var iCaretPos = 0;
        if (document.selection) {
            oField.focus();
            var oSel = document.selection.createRange();
            oSel.moveStart('character', -oField.value.length);
            iCaretPos = oSel.text.length;
        } else if (oField.selectionStart || oField.selectionStart == '0') {
            iCaretPos = oField.selectionStart;
        }
        return (iCaretPos);
    }

    /**
      * Returns boolean indicates whether user has pressed an expected key combination.
      * @param object keyPressEvent: JavaScript event object, refer
      *     http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
      * @param object lookupList: expected key combinations, as in:
      *     [13, {which: 188, shiftKey: true}]
      */
    function keyCombinationInList(keyPressEvent, lookupList) {
        var found = false;
        $.each(lookupList, function (index, keyCombination) {
            if (typeof (keyCombination) === 'number' && keyPressEvent.which === keyCombination) {
                found = true;
                return false;
            }

            if (keyPressEvent.which === keyCombination.which) {
                var alt = !keyCombination.hasOwnProperty('altKey') || keyPressEvent.altKey === keyCombination.altKey,
                    shift = !keyCombination.hasOwnProperty('shiftKey') || keyPressEvent.shiftKey === keyCombination.shiftKey,
                    ctrl = !keyCombination.hasOwnProperty('ctrlKey') || keyPressEvent.ctrlKey === keyCombination.ctrlKey;
                if (alt && shift && ctrl) {
                    found = true;
                    return false;
                }
            }
        });

        return found;
    }

    /**
     * Initialize tagsinput behaviour on inputs and selects which have
     * data-role=tagsinput
     */
    $(function () {
        $("input[data-role=tagsinput], select[multiple][data-role=tagsinput]").tagsinput();
    });
})(window.jQuery);
