/*
* Xphter.js
* This file defines all common functions provided by Xphter JavaScript Library.
* You must include this file in all pages that want to use functions defined in Xphter JavaScript Library.
*
* Namespace：Xphter
* Company: xphter.com
* Author: Du Peng
* QQ: 278770518
*
* Thinks!
*/

(function (global, undefined) {
    /*
    * Namespace Definition
    */
    var Xphter = global.Xphter = {};

    /***************** Append custom members to global object *****************/

    /*
    * Property Description:
    * Provides two properties for compatible of BOOLEAN type in object-oriented language.
    */
    var True = global.True = true;
    var False = global.False = false;

    /*
    * Method Description:
    * Gets element by ID.
    *
    * Parameters:
    * id: a string to indicate element ID.
    *
    * Return:
    * Returns the element with the specified ID or null if it is not existing.
    */
    var $ = global.$ = function (id) {
        if (DataTypeIdentity.IsUndefinedOrNull(id)) {
            return null;
        }

        var _id = id + String.Empty;
        return _id ? global.document.getElementById(_id) : null;
    };

    /*
    * Method Description:
    * Encodes the specified string which may contains some HTML markups.
    * These characters will be replaced:
    * >     ----------------> &gt;
    * <     ----------------> &lt;
    * &     ----------------> &amp;
    * "     ----------------> &quot;
    * space ----------------> &nbsp;
    *
    * Parameters:
    * html: a string will be encoded.
    *
    * Return:
    * Returns a string after encoding.
    */
    var HtmlEncode = global.HtmlEncode = function (html) {
        if (DataTypeIdentity.IsUndefinedOrNull(html)) {
            return html;
        }

        var character = null;
        var characters = [];
        var _html = html + String.Empty;
        for (var i = 0; i < _html.length; i++) {
            switch (character = _html.charAt(i)) {
                case ">":
                    characters.push("&gt;");
                    break;
                case "<":
                    characters.push("&lt;");
                    break;
                case "&":
                    characters.push("&amp;");
                    break;
                case "\"":
                    characters.push("&quot;");
                    break;
                case String.Space:
                    characters.push("&nbsp;");
                    break;
                default:
                    characters.push(character);
                    break;
            }
        }

        return characters.join(String.Empty);
    };

    /*
    * Method Description:
    * Encodes the specified string which may be a static string component of regular expression.
    * These characters will be replaced:
    * \     ----------------> \\
    * /     ----------------> \/
    * [     ----------------> \[
    * ]     ----------------> \]
    * (     ----------------> \(
    * )     ----------------> \)
    * {     ----------------> \{
    * }     ----------------> \}
    * |     ----------------> \|
    * ^     ----------------> \^
    * $     ----------------> \$
    * .     ----------------> \.
    * *     ----------------> \*
    * ?     ----------------> \?
    * +     ----------------> \+
    * =     ----------------> \=
    * !     ----------------> \!
    * :     ----------------> \:
    *
    * Parameters:
    * html: a string will be encoded.
    *
    * Return:
    * Returns a string after encoding.
    */
    var RegexEncode = global.RegexEncode = function (s) {
        if (DataTypeIdentity.IsUndefinedOrNull(s)) {
            return s;
        }

        var c = null;
        var result = String.Empty;
        var _s = s + String.Empty;
        for (var i = 0; i < _s.length; i++) {
            switch (c = _s.charAt(i)) {
                case "\\":
                case "/":
                case "[":
                case "]":
                case "(":
                case ")":
                case "{":
                case "}":
                case "|":
                case "^":
                case "$":
                case ".":
                case "*":
                case "?":
                case "+":
                case "=":
                case "!":
                case ":":
                    result += "\\" + c;
                    break;
                default:
                    result += c;
                    break;
            }
        }

        return result;
    };

    /*
    * Method Description:
    * Encodes the specified string to JSON format.
    * These characters will be replaced:
    * "     ----------------> \"
    * '     ----------------> \'
    * \     ----------------> \\
    *
    * Parameters:
    * value: a string will be encoded.
    *
    * Return:
    * Returns a string after encoding.
    */
    var StringEncode = global.StringEncode = function (value) {
        if (DataTypeIdentity.IsUndefinedOrNull(value)) {
            return value;
        }

        var c = null;
        var result = String.Empty;
        var text = value + String.Empty;
        for (var i = 0; i < text.length; i++) {
            switch (c = text.charAt(i)) {
                case "\"":
                case "\'":
                case "\\":
                    result += "\\" + c;
                    break;
                default:
                    result += c;
                    break;
            }
        }
        return result;
    };

    /*
    * Method Description:
    * Encodes the specified object to JSON format.
    *
    * Parameters:
    * value: The object to encode.
    * name: Object name.
    *
    * Return:
    * Returns a JSON string after encoding.
    */
    var JsonEncode = global.JsonEncode = function (value, name) {
        if (DataTypeIdentity.IsUndefinedOrNull(value)) {
            return String.Empty;
        }

        var result = DataTypeIdentity.IsUndefinedOrNull(name) ? String.Empty : String.Format("\"{0}\":", StringEncode(name));
        if (DataTypeIdentity.IsNumber(value)) {
            result += value.toString();
        } else if (DataTypeIdentity.IsString(value)) {
            result += String.Format("\"{0}\"", StringEncode(value));
        } else if (DataTypeIdentity.IsBoolean(value)) {
            result += value.toString();
        } else if (DataTypeIdentity.IsObject(value)) {
            if (DataTypeIdentity.IsArray(value)) {
                result += "[";
                var item = null;
                for (var i = 0; i < value.length; i++) {
                    if (item = JsonEncode(value[i])) {
                        result += item;
                        ++i;
                        break;
                    }
                }
                for (; i < value.length; i++) {
                    if (item = JsonEncode(value[i])) {
                        result += String.Format(",{0}", item);
                    }
                }
                result += "]";
            } else if (DataTypeIdentity.IsDateTime(value)) {
                throw new Error("Not support to encode Date object to JSON format.");
            } else {
                result += "{";
                var item = null;
                var properties = [];
                for (var p in value) {
                    properties.push(p);
                }
                for (var i = 0; i < properties.length; i++) {
                    if (item = JsonEncode(value[properties[i]], properties[i])) {
                        result += item;
                        ++i;
                        break;
                    }
                }
                for (; i < properties.length; i++) {
                    if (item = JsonEncode(value[properties[i]], properties[i])) {
                        result += String.Format(",{0}", item);
                    }
                }
                result += "}";
            }
        }

        return result;
    };

    /*
    * Method Description:
    * Executes the specified code text in global scope.
    *
    * Parameters:
    * code: The code text.
    *
    * Return:
    * Returns the result after execution.
    */
    var Execute = global.Execute = function (code) {
        if (!code) {
            throw new Error("code text is undefined.");
        }

        var _code = code + String.Empty;

        if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9) {
            return global.execScript(_code);
        } else {
            return global.eval(_code);
        }
    };

    /***************** Expand Array Class *****************/

    /*
    * Method Description:
    * Determines whether this array contains the sepcified object.
    *
    * Return:
    * Returns true if this array contains the specified object, otherwise false.
    */
    Array.prototype.Contains = function (obj, isAllowTransform) {
        if (DataTypeIdentity.IsUndefined(obj)) {
            throw new Error("obj is undefined.");
        }

        var result = false;
        var _isAllowTransform = !!isAllowTransform;
        for (var i = 0; i < this.length; i++) {
            if (_isAllowTransform && this[i] == obj || !_isAllowTransform && this[i] === obj) {
                result = true;
                break;
            }
        }
        return result;
    };

    /*
    * Method Description:
    * Adds the sepcified array to this array.
    *
    * Return:
    * Returns the array self.
    */
    Array.prototype.AddRange = function (data) {
        if (!DataTypeIdentity.IsArray(data)) {
            throw new Error("data is not a array.");
        }

        for (var i = 0; i < data.length; i++) {
            this.push(data[i]);
        }

        return this;
    };

    /*
    * Method Description:
    * Removes the sepcified object from this array.
    *
    * Return:
    * Returns true if this array contains the specified object and has remove it, otherwise false.
    */
    Array.prototype.Remove = function (obj, isAllowTransform) {
        if (DataTypeIdentity.IsUndefined(obj)) {
            throw new Error("obj is undefined.");
        }

        var result = false;
        var _isAllowTransform = !!isAllowTransform;
        for (var i = 0; i < this.length; i++) {
            if (_isAllowTransform && this[i] == obj || !_isAllowTransform && this[i] === obj) {
                this.splice(i, 1);
                result = true;
                break;
            }
        }
        return result;
    };

    /*
    * Method Description:
    * Removes element at the specified position from this array.
    * 
    * Parameters:
    * index: the element index.
    */
    Array.prototype.RemoveAt = function (index) {
        var _index = 0;
        if (!DataTypeIdentity.IsUndefinedOrNull(index)) {
            _index = index - 0 || 0;
        }
        if (_index < 0 || _index >= this.length) {
            throw new Error("index is out of range.");
        }

        for (var i = _index + 1; i < this.length; i++) {
            this[i - 1] = this[i];
        }
        this.length = this.length - 1;
    };

    /*
    * Method Description:
    * Exchanges values in the sepcified indices from this array.
    *
    * Parameters:
    * index1: The position of first element.
    * index2: The position of second element.
    *
    */
    Array.prototype.Exchange = function (index1, index2) {
        if (global.isNaN(index1)) {
            throw new Error("index1 is not a number.");
        }
        if (global.isNaN(index2)) {
            throw new Error("index2 is not a number.");
        }
        index1 = index1 - 1;
        index2 = index2 - 1;
        if (index1 < 0 || index1 >= this.length) {
            throw new Error("index1 is out of range.");
        }
        if (index2 < 0 || index2 >= this.length) {
            throw new Error("index2 is out of range.");
        }

        var temp = this[index1];
        this[index1] = this[index2];
        this[index2] = temp;
    };

    /*
    * Method Description:
    * Gets index of the sepcified object.
    *
    * Return:
    * Returns the index of the specified object, if it is not existing return -1.
    */
    Array.prototype.IndexOf = function (obj, isAllowTransform) {
        if (DataTypeIdentity.IsUndefined(obj)) {
            throw new Error("obj is undefined.");
        }

        var index = -1;
        var _isAllowTransform = !!isAllowTransform;
        for (var i = 0; i < this.length; i++) {
            if (_isAllowTransform && this[i] == obj || !_isAllowTransform && this[i] === obj) {
                index = i;
                break;
            }
        }
        return index;
    };

    /*
    * Method Description:
    * Filters elements based on a predicate.
    *
    * Parameters:
    * predicate: A function to test each element for a condition.
    *
    * Return:
    * Returns a new array that contains elements from this array that satisfy the condition.
    */
    Array.prototype.Where = function (predicate) {
        if (!DataTypeIdentity.IsFunction(predicate)) {
            throw new Error("The function to test each element for a condition is undefined.");
        }

        var result = [];
        var element = null;
        for (var i = 0; i < this.length; i++) {
            if (predicate(element = this[i], i)) {
                result.push(element);
            }
        }
        return result;
    };

    /*
    * Method Description:
    * Determines whether has a element match the specified predicate.
    *
    * Parameters:
    * predicate: A function to test each element for a condition.
    *
    * Return: true or false.
    */
    Array.prototype.Any = function (predicate) {
        if (!DataTypeIdentity.IsFunction(predicate)) {
            throw new Error("The function to test each element for a condition is undefined.");
        }

        var element = null;
        for (var i = 0; i < this.length; i++) {
            if (predicate(element = this[i], i)) {
                return true;
            }
        }
        return false;
    };

    /*
    * Method Description:
    * Determines whether all elements match the specified predicate.
    *
    * Parameters:
    * predicate: A function to test each element for a condition.
    *
    * Return: true or false.
    */
    Array.prototype.All = function (predicate) {
        if (!DataTypeIdentity.IsFunction(predicate)) {
            throw new Error("The function to test each element for a condition is undefined.");
        }

        var element = null;
        for (var i = 0; i < this.length; i++) {
            if (!predicate(element = this[i], i)) {
                return false;
            }
        }
        return true;
    };

    /*
    * Method Description:
    * Orders elements based on a predicate.
    *
    * Parameters:
    * selector: A function to get a value to compare form each element.
    *
    * Return:
    * Returns a new array that contains ordered elements from this array.
    */
    Array.prototype.OrderBy = function (selector) {
        if (!this.length) {
            return [];
        } else if (this.length == 1) {
            return [this[0]];
        }

        if (!DataTypeIdentity.IsFunction(selector)) {
            selector = function (item) {
                return item - 0;
            };
        }

        var item = null;
        var result = this.Copy();
        for (var i = 0; i < result.length - 1; i++) {
            for (var j = i + 1; j < result.length; j++) {
                if (selector(result[j]) < selector(result[i])) {
                    item = result[i];
                    result[i] = result[j];
                    result[j] = item;
                }
            }
        }

        return result;
    };

    /*
    * Method Description:
    * Groups elements by a predicate.
    *
    * Parameters:
    * selector: A function to get the group string key of each element.
    *
    * Return:
    * Returns a new array that contains ordered elements from this array.
    */
    Array.prototype.GroupBy = function (selector) {
        if (!this.length) {
            return {};
        }

        if (!DataTypeIdentity.IsFunction(selector)) {
            selector = function (item) {
                return item;
            };
        }

        var result = {};
        var key = null, item = null;
        for (var i = 0; i < this.length; i++) {
            if (DataTypeIdentity.IsUndefinedOrNull(item = this[i])) {
                continue;
            }
            if (DataTypeIdentity.IsUndefinedOrNull(key = selector(item))) {
                continue;
            }

            key = key.toString();
            if (!(key in result)) {
                result[key] = [];
            }
            result[key].push(item);
        }

        return result;
    };

    /*
    * Method Description:
    * Filters all repeated elements in this array.
    *
    * Return:
    * Returns a new array which contains all distinct elements from this array.
    */
    Array.prototype.Distinct = function () {
        if (!this.length) {
            return [];
        } else if (this.length == 1) {
            return [this[0]];
        }

        var item = null;
        var result = [this[0]];
        var isMatch = false;
        for (var i = 1; i < this.length; i++) {
            isMatch = true;
            item = this[i];

            for (var j = 0; j < result.length; j++) {
                if (result[j] === item) {
                    isMatch = false;
                    break;
                }
            }

            isMatch && result.push(item);
        }
        return result;
    };

    /*
    * Method Description:
    * Projects each element of this array to a new array.
    *
    * Parameters:
    * selector: A transform function to apply each element.
    *
    * Return:
    * Returns a new array whose elements are the result of invoking the transform function on each element of this array.
    */
    Array.prototype.Select = function (selector) {
        if (!DataTypeIdentity.IsFunction(selector)) {
            throw new Error("The transform function to apply each element is undefined.");
        }

        var result = [];
        for (var i = 0; i < this.length; i++) {
            result.push(selector(this[i], i));
        }
        return result;
    };


    /*
    * Method Description:
    * Applies an accumulator function over a array.
    *
    * Parameters:
    * func: A accumulator function to apply each element.
    *
    * Return:
    * Returns the accumulator result of this array.
    */
    Array.prototype.Aggregate = function (func) {
        if (!DataTypeIdentity.IsFunction(func)) {
            throw new Error("The accumulator function to apply each element is undefined.");
        }
        if (!this.length) {
            throw new Error("The array is empty.");
        }

        if (this.length == 1) {
            return this[0];
        }

        var result = func(this[0], this[1]);
        for (var i = 2; i < this.length; i++) {
            result = func(result, this[i]);
        }
        return result;
    };

    /*
    * Method Description:
    * Searches an element based on a predicate.
    *
    * Parameters:
    * predicate: A function to test each element for a condition.
    *
    * Return:
    * Returns the index of the first element of this array that satisfy the condition, if found; otherwise, -1.
    */
    Array.prototype.FindIndex = function (predicate) {
        if (!DataTypeIdentity.IsFunction(predicate)) {
            throw new Error("The function to test each element for a condition is undefined.");
        }

        for (var i = 0; i < this.length; i++) {
            if (predicate(this[i], i)) {
                return i;
            }
        }
        return -1;
    };

    /*
    * Method Description:
    * Reverses items in this array.
    *
    * Return:
    * Returns a new array that contains reversed elements from this array.
    */
    Array.prototype.Reverse = function () {
        if (!this.length) {
            return [];
        }

        var result = [];
        for (var i = this.length - 1; i >= 0; i--) {
            result.push(this[i]);
        }
        return result;
    };

    /*
    * Method Description:
    * Copys all items in this array.
    *
    * Return:
    * Returns a new array that contains all elements from this array.
    */
    Array.prototype.Copy = function () {
        if (!this.length) {
            return [];
        }

        var result = [];
        for (var i = 0; i < this.length; i++) {
            result.push(this[i]);
        }
        return result;
    };

    /*
    * Method Description:
    * Performs the specified action on each element of this array.
    *
    * Parameters:
    * predicate: A function to perform on each element of this array.
    *
    */
    Array.prototype.ForEach = function (action) {
        if (!DataTypeIdentity.IsFunction(action)) {
            throw new Error("The function to perform on each element of this array is undefined.");
        }

        for (var i = 0; i < this.length; i++) {
            action(this[i], i);
        }
    };

    /*
    * Method Description:
    * Computes the number sum of all elements.
    *
    * Parameters:
    * valueSelector: A function to fetch a number value from each element.
    *
    * Return:
    * Returns the number sum of all element.
    */
    Array.prototype.Sum = function (valueSelector) {
        if (!DataTypeIdentity.IsFunction(valueSelector)) {
            valueSelector = function (item) {
                return item - 0;
            };
        }

        var result = 0;
        var item = null;
        for (var i = 0; i < this.length; i++) {
            if (DataTypeIdentity.IsUndefinedOrNull(item = this[i])) {
                continue;
            }

            result += (valueSelector(item, i) - 0);
        }

        return result;
    };

    /*
    * Method Description:
    * Finds the maximum number value of all elements.
    *
    * Parameters:
    * valueSelector: A function to fetch a number value from each element.
    *
    * Return:
    * Returns the maximum number value of all element.
    */
    Array.prototype.Max = function (valueSelector) {
        if (!DataTypeIdentity.IsFunction(valueSelector)) {
            valueSelector = function (item) {
                return item - 0;
            };
        }

        if (!this.length) {
            return null;
        }

        var result = Number.MIN_VALUE;
        var item = null, value = null;
        for (var i = 0; i < this.length; i++) {
            if (DataTypeIdentity.IsUndefinedOrNull(item = this[i])) {
                continue;
            }

            if ((value = valueSelector(item, i)) > result) {
                result = value;
            }
        }

        return result;
    };

    /*
    * Method Description:
    * Finds the minimum number value of all elements.
    *
    * Parameters:
    * valueSelector: A function to fetch a number value from each element.
    *
    * Return:
    * Returns the minimum number value of all element.
    */
    Array.prototype.Min = function (valueSelector) {
        if (!DataTypeIdentity.IsFunction(valueSelector)) {
            valueSelector = function (item) {
                return item - 0;
            };
        }

        if (!this.length) {
            return null;
        }

        var result = Number.MAX_VALUE;
        var item = null, value = null;
        for (var i = 0; i < this.length; i++) {
            if (DataTypeIdentity.IsUndefinedOrNull(item = this[i])) {
                continue;
            }

            if ((value = valueSelector(item, i)) < result) {
                result = value;
            }
        }

        return result;
    };

    /*
    * Method Description:
    * Gets the first element of this array.
    *
    * Return:
    * Returns the first element of this array or null if it is not existing.
    */
    Array.prototype.FirstOrNull = function () {
        return this.length > 0 ? this[0] : null;
    };

    /*
    * Method Description:
    * Gets the first element of this array.
    *
    * Return:
    * Returns the first element of this array or the default value if it is not existing.
    */
    Array.prototype.FirstOrDefault = function (defaultValue) {
        return this.length > 0 ? this[0] : (DataTypeIdentity.IsUndefined(defaultValue) ? null : defaultValue);
    };

    /*
    * Method Description:
    * Gets the last element of this array.
    *
    * Return:
    * Returns the last element of this array or null if it is not existing.
    */
    Array.prototype.LastOrNull = function () {
        return this.length > 0 ? this[this.length - 1] : null;
    };

    /***************** Expand String Class *****************/

    /*
    * Static Property Description:
    * Gets a empty string.
    */
    String.Empty = "";

    /*
    * Static Property Description:
    * Gets a space string.
    */
    String.Space = " ";

    /*
    * Static Property Description:
    * Gets a regular expression to match a integer.
    */
    String.IntegerRegex = /^\s*[\-\+]?(?:0x)?\d+\s*$/i;

    /*
    * Static Property Description:
    * Gets a regular expression to match a number.
    */
    String.NumberRegex = /^\s*[\-\+]?(?:0x)?\d+(?:\.\d*)?\s*$/i;

    /*
    * Static Property Description:
    * Gets a regular expression to match a mobile number only in china.
    */
    String.PhoneNumberRegex = /^\s*(?:0?86)?-?1\d{10}\s*$/i;

    /*
    * Static Property Description:
    * Gets a regular expression to match a IPv4 address.
    */
    String.IPv4Regex = /^\s*(?:1\d\d|2[0-4]\d|25[0-5]|[1-9]\d|\d)(?:\.(?:1\d\d|2[0-4]\d|25[0-5]|[1-9]\d|\d)){3}\s*$/i;

    /*
    * Static Method Description:
    * Replaces each format item in a specified string with the text equivalent of a corresponding object's value.
    */
    String.Format = function (format) {
        if (DataTypeIdentity.IsUndefinedOrNull(format)) {
            throw Error("String format is undefined.");
        }
        var _format = format + String.Empty;
        if (_format.IsEmptyOrWhiteSpace()) {
            return _format;
        }

        var match = null;
        var matches = [];
        var regex = /\{(\d+)\}/g;
        while ((match = regex.exec(_format)) != null) {
            matches.push(match);
        }

        var index = 0;
        var endIndex = 0;
        var previousIndex = 0;
        var nextIndex = 0;
        var prefixRegex = /\{+$/g;
        var postfixRegex = /^\}+/g;
        var prefixMatch = null;
        var postfixMatch = null;
        for (var i = 0; i < matches.length; i++) {
            index = (match = matches[i]).index;
            endIndex = index + match[0].length;
            nextIndex = i < matches.length - 1 ? matches[i + 1].index : _format.length;

            prefixRegex.lastIndex = postfixRegex.lastIndex = 0;
            prefixMatch = previousIndex < index ? prefixRegex.exec(_format.substring(previousIndex, index)) : null;
            postfixMatch = endIndex < nextIndex ? postfixRegex.exec(_format.substring(endIndex, nextIndex)) : null;
            if (prefixMatch != null && prefixMatch[0].length % 2 == 1 || postfixMatch != null && postfixMatch[0].length % 2 == 1) {
                matches.splice(i--, 1);
            } else {
                previousIndex = endIndex;
            }
        }

        for (var i = 0; i < matches.length; i++) {
            if (global.parseInt(matches[i][1]) > arguments.length - 2) {
                throw Error("The index of format item is out of range.");
            }
        }

        var value = null;
        var startIndex = 0;
        var toIndex = 0;
        regex = /\{\{|\}\}/g;
        var result = []
        for (var i = 0; i < matches.length; i++) {
            value = arguments[global.parseInt((match = matches[i])[1]) + 1];
            if (startIndex < (toIndex = match.index)) {
                result.push(_format.substring(startIndex, toIndex).replace(regex, function () {
                    return arguments[0].charAt(0);
                }));
            }
            if (!DataTypeIdentity.IsUndefinedOrNull(value)) {
                result.push(value + String.Empty);
            }
            startIndex = toIndex + match[0].length;
        }
        if (startIndex < _format.length) {
            result.push(_format.substring(startIndex, _format.length).replace(regex, function () {
                return arguments[0].charAt(0);
            }));
        }

        return result.join(String.Empty);
    };

    /*
    * Static Method Description:
    * Returns s repeated n times.
    */
    String.Repeat = function (s, n) {
        var _s = s + String.Empty;
        var _n = n - 0;
        if (global.isNaN(_n)) {
            throw new Error(n + " is not a number.");
        }

        var text = String.Empty;
        for (var i = 0; i < _n; i++) {
            text += _s;
        }

        return text;
    };

    /*
    * Method Description:
    * Determines whether this string equals the sepcified string, ignore case.
    *
    * Return:
    * Returns true if this string equals the specified string, otherwise false.
    */
    String.prototype.EqualsIgnoreCase = function (s) {
        return DataTypeIdentity.IsString(s) ? this.toLowerCase() == s.toLowerCase() : false;
    };

    /*
    * Method Description:
    * Trims white characters from the start of this string.
    *
    * Return:
    * Returns a new string trimed white characters from the start of this string.
    */
    String.prototype.TrimStart = function () {
        return this.replace(/^\s+/, String.Empty);
    };

    /*
    * Method Description:
    * Trims white characters from the end of this string.
    *
    * Return:
    * Returns a new string trimed white characters from the end of this string.
    */
    String.prototype.TrimEnd = function () {
        return this.replace(/\s+$/, String.Empty);
    };

    /*
    * Method Description:
    * Trims white characters from this string.
    *
    * Return:
    * Returns a new string trimed white characters from this string.
    */
    String.prototype.Trim = function () {
        return this.replace(/(?:^\s+)|(?:\s+$)/g, String.Empty);
    };

    /*
    * Method Description:
    * Determines whether this string is empty.
    *
    * Return:
    * Returns true if the specified string is empty, otherwise return false.
    */
    String.prototype.IsEmpty = function () {
        return this.length == 0;
    };

    /*
    * Method Description:
    * Determines whether this string is empty or only contains white characters.
    *
    * Return:
    * Returns true if the specified string is empty or only contains white characters, otherwise return false.
    */
    String.prototype.IsEmptyOrWhiteSpace = function () {
        return (/^\s*$/g).test(this);
    };

    /*
    * Method Description:
    * Determines whether this string starts with the specified string.
    *
    * Return:
    * Returns true if this string starts with the specified string, otherwise return false.
    */
    String.prototype.StartsWith = function (value) {
        return !!(value && new RegExp("^" + global.RegexEncode(value), "g").test(this));
    };

    /*
    * Method Description:
    * Determines whether this string ends with the specified string.
    *
    * Return:
    * Returns true if this string ends with the specified string, otherwise return false.
    */
    String.prototype.EndsWith = function (value) {
        return !!(value && new RegExp(global.RegexEncode(value) + "$", "g").test(this));
    };

    /*
    * Method Description:
    * Replace all the specified string in this string with the specified string.
    *
    * Return:
    * Returns a string after replaced.
    */
    String.prototype.ReplaceGlobal = function (value, replacement) {
        return this.replace(new RegExp(global.RegexEncode(value), "g"), replacement);
    };

    /*
    * Method Description:
    * Determines whether this string represents a e-mail address.
    *
    * Return:
    * Returns true if this string represents a e-mail address, otherwise return false.
    */
    String.prototype.IsEmail = function () {
        return !this.IsEmptyOrWhiteSpace() && new MailAddress(this).GetIsWellFormat();
    };

    /*
    * Method Description:
    * Determines whether this string represents a URI.
    *
    * Return:
    * Returns true if this string represents a URI, otherwise return false.
    */
    String.prototype.IsUri = function () {
        return !this.IsEmptyOrWhiteSpace() && new Uri(this).GetIsWellFormat();
    };

    /*
    * Method Description:
    * Determines whether this string represents a integer.
    *
    * Return:
    * Returns true if this string represents a integer, otherwise return false.
    */
    String.prototype.IsInteger = function () {
        if (this.IsEmptyOrWhiteSpace()) {
            return false;
        }

        return String.IntegerRegex.test(this);
    };

    /*
    * Method Description:
    * Determines whether this string represents a number.
    *
    * Return:
    * Returns true if this string represents a number, otherwise return false.
    */
    String.prototype.IsNumber = function () {
        if (this.IsEmptyOrWhiteSpace()) {
            return false;
        }

        return String.NumberRegex.test(this);
    };

    /*
    * Method Description:
    * Determines whether this string represents a DateTime.
    *
    * Return:
    * Returns true if this string represents a DateTime, otherwise return false.
    */
    String.prototype.IsDateTime = function () {
        if (this.IsEmptyOrWhiteSpace()) {
            return false;
        }

        return !global.isNaN(Date.parse(this));
    };

    /*
    * Method Description:
    * Determines whether this string represents a mobile number.
    *
    * Return:
    * Returns true if this string represents a mobile number, otherwise return false.
    */
    String.prototype.IsPhoneNumber = function () {
        if (this.IsEmptyOrWhiteSpace()) {
            return false;
        }

        return String.PhoneNumberRegex.test(this);
    };

    /*
    * Method Description:
    * Determines whether this string represents a IPv4 address.
    *
    * Return:
    * Returns true if this string represents a IPv4 address, otherwise return false.
    */
    String.prototype.IsIPv4 = function () {
        if (this.IsEmptyOrWhiteSpace()) {
            return false;
        }

        return String.IPv4Regex.test(this);
    };

    /***************** Expand Number Class *****************/

    /*
    * Internal method Description:
    * Checks arguments whether all items is number.
    */
    Number._CheckArguments = function () {
        for (var i = 0; i < arguments.length; i++) {
            if (global.isNaN(arguments[i])) {
                throw new Error(arguments[i] + " is not a number");
            }
        }
    };

    /*
    * Internal method Description:
    * Gets the number of decimal places of the speicifed number.
    */
    Number._GetDecimalPlaces = function (n) {
        var parts = n.toString().split(".");
        return parts.length > 1 ? parts[1].length : 0;
    };

    /*
    * Internal method Description:
    * Moves the decimal point left or right.
    */
    Number._MoveDecimalPoint = function (number, count) {
        if (count == 0) {
            return number;
        }

        var sign = String.Empty;
        if (number < 0) {
            sign = "-";
        }
        number = Math.abs(number).toString();

        var decimalPlaces = 0;
        var integerPlaces = number.indexOf(".");
        if (integerPlaces < 0) {
            integerPlaces = number.length;
        } else {
            decimalPlaces = number.length - integerPlaces - 1;
        }
        var chars = number.split(String.Empty);

        chars.splice(integerPlaces, 1);
        if (count > 0) {
            if (count > decimalPlaces) {
                for (var i = 0; i < count - decimalPlaces; i++) {
                    chars.push("0");
                }
            } else if (count == decimalPlaces) {
            } else {
                chars.splice(integerPlaces + count, 0, ".");
            }
        } else {
            count = Math.abs(count);
            if (count > integerPlaces) {
                for (var i = 0; i < count - integerPlaces; i++) {
                    chars.unshift("0");
                }
                chars.unshift("0.");
            } else if (count == integerPlaces) {
                chars.unshift("0.");
            } else {
                chars.splice(integerPlaces - count, 0, ".");
            }
        }

        chars.unshift(sign);
        return Number(new String(chars.join(String.Empty)));
    };

    /*
    * Static Method Description:
    * Adds n1 with n2.
    */
    Number.Add = function (n1, n2) {
        var _n1 = n1 - 0, _n2 = n2 - 0;
        Number._CheckArguments(_n1, _n2);
        if (_n2 == 0) {
            return _n1;
        }

        var p1 = Number._GetDecimalPlaces(_n1);
        var p2 = Number._GetDecimalPlaces(_n2);
        if (p1 == 0 && p2 == 0) {
            return _n1 + _n2;
        }

        var mp = Math.max(p1, p2);
        _n1 = Number._MoveDecimalPoint(_n1, mp);
        _n2 = Number._MoveDecimalPoint(_n2, mp);
        return Number._MoveDecimalPoint(_n1 + _n2, -mp);
    };

    /*
    * Static Method Description:
    * Subtracts n1 with n2.
    */
    Number.Subtract = function (n1, n2) {
        var _n1 = n1 - 0, _n2 = n2 - 0;
        Number._CheckArguments(_n1, _n2);
        if (_n2 == 0) {
            return _n1;
        }

        if (_n2 > 0) {
            _n2 = Number("-" + _n2.toString());
        } else if (_n2 < 0) {
            _n2 = Number(_n2.toString().substr(1));
        }

        return Number.Add(_n1, _n2);
    };

    /*
    * Static Method Description:
    * Multiplies n1 and n2.
    */
    Number.Multiply = function (n1, n2) {
        var _n1 = n1 - 0, _n2 = n2 - 0;
        Number._CheckArguments(_n1, _n2);
        if (_n1 == 0 || _n2 == 0) {
            return 0;
        }
        if (_n1 == 1) {
            return _n2;
        }
        if (_n1 == -1) {
            return -_n2;
        }
        if (_n2 == 1) {
            return _n1;
        }
        if (_n2 == -1) {
            return -_n1;
        }

        var p1 = Number._GetDecimalPlaces(_n1);
        var p2 = Number._GetDecimalPlaces(_n2);
        if (p1 == 0 && p2 == 0) {
            return _n1 * _n2;
        }

        _n1 = Number._MoveDecimalPoint(_n1, p1);
        _n2 = Number._MoveDecimalPoint(_n2, p2);
        return Number._MoveDecimalPoint(_n1 * _n2, -(p1 + p2));
    };


    /***************** Expand Date Class *****************/

    /*
    * Method Description:
    * Determine whether this is a leap year.
    *
    * Return:
    * Returns a string after replaced.
    */
    Date.IsLeapYear = function (year) {
        return (year % 100 == 0) ? (year % 400 == 0) : (year % 4 == 0);
    };

    Date._dayNumbers = {
        "0": 31,
        "2": 31,
        "3": 30,
        "4": 31,
        "5": 30,
        "6": 31,
        "7": 31,
        "8": 30,
        "9": 31,
        "10": 30,
        "11": 31
    };

    /*
    * Method Description:
    * Get number of day in the specified month.
    *
    * Return:
    * Returns a string after replaced.
    */
    Date.GetDayCount = function (year, month) {
        if (month < 0 || month > 11) {
            throw new Error("Month is less 0 or greater than 11.");
        }

        return month == 1 ? (Date.IsLeapYear(year) ? 29 : 28) : Date._dayNumbers[month];
    };

    /*
    * Method Description:
    * Creates a Date value from the specified json text generated by .net JavaScriptSerializer class.
    *
    * Return:
    * Returns a string after replaced.
    */
    Date.FromNetJson = function (json) {
        if (!json) {
            throw new Error("The JSON text is empty.");
        }

        return new Date(global.parseInt(/^\/Date\((\d+)\)\/$/i.exec(json)[1]));
    };

    /*
    * The weekday (short) name array.
    */
    Date.WeekdayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    Date.WeekdayShortNames = ["Sun", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat"];

    /*
    * The month (short) name array.
    */
    Date.MonthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    Date.MonthShortNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec"];

    /*
    * Method Description:
    * Gets a value to indicate whether current Date equals the specified object.
    *
    * Return:
    * Returns a string represents the weekday.
    */
    Date.prototype.Equals = function (obj) {
        if (!(obj instanceof Date)) {
            return false;
        }

        return this.getTime() === obj.getTime();
    };

    /*
    * Method Description:
    * Gets the weekday name of this Date object.
    *
    * Return:
    * Returns a string represents the weekday.
    */
    Date.prototype.GetWeekdayName = function () {
        return Date.WeekdayNames[this.getDay()];
    };

    /*
    * Method Description:
    * Gets the weekday short name of this Date object.
    *
    * Return:
    * Returns a string represents the weekday.
    */
    Date.prototype.GetWeekdayShortName = function () {
        return Date.WeekdayShortNames[this.getDay()];
    };

    /*
    * Method Description:
    * Gets the month name of this Date object.
    *
    * Return:
    * Returns a string represents the month name.
    */
    Date.prototype.GetMonthName = function () {
        return Date.MonthNames[this.getMonth()];
    };

    /*
    * Method Description:
    * Gets the month short name of this Date object.
    *
    * Return:
    * Returns a string represents the month short name.
    */
    Date.prototype.GetMonthShortName = function () {
        return Date.MonthShortNames[this.getMonth()];
    };

    /***************** DataTypeNames Class Definition *****************/

    /*
    * Static Class Description:
    * Represents all data type names used in JavaScript runtime.
    */
    var DataTypeNames = Xphter.DataTypeNames = {
        Undefined: "undefined",
        Number: "number",
        String: "string",
        Boolean: "boolean",
        Function: "function",
        Object: "object"
    };

    /***************** DomNodeTypes Class Definition *****************/

    /*
    * Static Class Description:
    * Represents all node type in 1 Level DOM.
    */
    var DomNodeTypes = Xphter.DomNodeTypes = {
        Element: 1,
        Attribute: 2,
        Text: 3,
        CDDataSection: 4,
        ProcessingInstruction: 7,
        Comment: 8,
        Document: 9,
        DocumentType: 10,
        DocumentFragment: 11
    };

    /***************** DataTypeIdentity Class Definition *****************/

    /*
    * Static Class Description:
    * Provide functions to identity data type.
    */
    var DataTypeIdentity = Xphter.DataTypeIdentity = {
        /*
        * Method Description:
        * Determines whether the specified value is a number.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a number otherwise false.
        */
        IsNumber: function (value) {
            return typeof value == DataTypeNames.Number;
        },

        /*
        * Method Description:
        * Determines whether the specified value is a integer.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a integer otherwise false.
        */
        IsInteger: function (value) {
            return (typeof value == DataTypeNames.Number) && (Math.floor(value) == value);
        },

        /*
        * Method Description:
        * Determines whether the specified value is a string.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a string otherwise false.
        */
        IsString: function (value) {
            return typeof value == DataTypeNames.String;
        },

        /*
        * Method Description:
        * Determines whether the specified value is a boolean.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a boolean otherwise false.
        */
        IsBoolean: function (value) {
            return typeof value == DataTypeNames.Boolean;
        },

        /*
        * Method Description:
        * Determines whether the specified value is a function.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a function otherwise false.
        */
        IsFunction: function (value) {
            return typeof value == DataTypeNames.Function;
        },

        /*
        * Method Description:
        * Determines whether the specified value is a object.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a object otherwise false.
        */
        IsObject: function (value) {
            return typeof value == DataTypeNames.Object;
        },

        /*
        * Method Description:
        * Determines whether the specified value is not null and is a object.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a non-null object otherwise false.
        */
        IsNotNullObject: function (value) {
            return typeof value == DataTypeNames.Object && value !== null;
        },

        /*
        * Method Description:
        * Determines whether the specified value is undefined.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is undefined otherwise false.
        */
        IsUndefined: function (value) {
            return value === undefined;
        },

        /*
        * Method Description:
        * Determines whether the specified value is undefined or null.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is undefined or null otherwise false.
        */
        IsUndefinedOrNull: function (value) {
            return value === undefined || value === null;
        },

        /*
        * Method Description:
        * Determines whether the specified value is a array.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a array otherwise false.
        */
        IsArray: function (value) {
            return DataTypeIdentity.IsNotNullObject(value) && ((value instanceof Array) || (Object.prototype.toString.call(value) === "[object Array]"));
        },

        /*
        * Method Description:
        * Determines whether the specified value is a DateTime.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value is a DateTime otherwise false.
        */
        IsDateTime: function (value) {
            return DataTypeIdentity.IsNotNullObject(value) && (value instanceof Date);
        }
    };

    /***************** IInterface Class Definition *****************/

    /*
    * Static Class Description:
    * Represents the base class of all interface.
    *
    * Mark:
    * Define a interface as below:
    * IA = {
    *   IsImplementBy : IInterface.IsImplementBy,
    *
    *   InterfaceMethod : function { throw new Error("Not Implementation."); }
    * }
    *
    * Check whether a object has implemented this interface as below:
    * var hasImplemented = IA.IsImplementBy(obj);
    */
    var IInterface = Xphter.IInterface = {
        /*
        * Method Description:
        * Determines whether the specified value implements this interface.
        *
        * Parameters:
        * value: The value will be check.
        *
        * Return:
        * Returns true if the specified value implements this interface otherwise false.
        */
        IsImplementBy: function (value) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                return false;
            }

            var result = true;
            var classMethod = null;
            var interfaceMethod = null;
            for (var name in this) {
                if (!result) {
                    break;
                }
                if (!DataTypeIdentity.IsFunction(interfaceMethod = this[name])) {
                    continue;
                }
                if (interfaceMethod === arguments.callee) {
                    continue;
                }

                classMethod = value[name];
                result = DataTypeIdentity.IsFunction(classMethod) && classMethod.length === interfaceMethod.length;
            }

            return result;
        }
    };

    /***************** MailAddress Class Definition *****************/

    /*
    * Class Description:
    * Represents a e-mail address.
    *
    * Constructor Parameters:
    * originalString : The string of e-mail address;
    */
    var MailAddress = Xphter.MailAddress = function (originalString) {
        var m_originalString = null;
        var m_username = null;
        var m_hostname = null;

        var m_isWellFormat = false;
        var m_regex = /^([\w\.\-]+)\@((?:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})|(?:[\w\-]+(?:\.[\w\-]+)+))$/i;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (DataTypeIdentity.IsUndefinedOrNull(originalString)) {
                throw new Error("Mail address string is undefined.");
            }
            if (!(m_originalString = originalString + String.Empty)) {
                return;
            }
            var match = m_regex.exec(m_originalString);
            if (match) {
                m_username = match[1];
                m_hostname = match[2];
                m_isWellFormat = true;
            }
        }

        /*
        * Method Description:
        * Gets the originalString parameter.
        */
        this.GetOriginalString = function () {
            return m_originalString;
        };

        /*
        * Method Description:
        * Gets whether the originalString parameter is a a well format e-mail string.
        */
        this.GetIsWellFormat = function () {
            return m_isWellFormat;
        };

        /*
        * Method Description:
        * Gets username of this e-mail address.
        */
        this.GetUsername = function () {
            return m_username;
        };

        /*
        * Method Description:
        * Gets hostname of this e-mail address.
        */
        this.GetHostname = function () {
            return m_hostname;
        };
    }

    /***************** Uri Class Definition *****************/

    /*
    * Class Description:
    * Represents a URI.
    *
    * Constructor Parameters:
    * originalString : The string of URI;
    * isRelative: Determine whether the URI string represents a relative URI. If this parameter is missing, then class will identify whether it is a relative URI.
    */
    var Uri = Xphter.Uri = function (originalString, isRelative) {
        var m_originalString = null;
        var m_isRelative = null;
        var m_schema = null;
        var m_hostname = null;
        var m_port = null;
        var m_path = null;
        var m_fragment = null;
        var m_query = null;

        var m_isWellFormat = false;
        var m_absoluteRegex = /^([\w\.\-]+)\:\/\/((?:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})|(?:[\w\-]+(?:\.[\w\-]+)+)|(?:localhost))(?:\:(\d+))?((?:\/[\w\,\.\-\%\+]*)*)(?:(\#[\w\,\.\-\%\&\;\=\*\!\~\(\)]+)|(\?[\w\,\.\-\%\&\;\=\*\!\~\(\)\/]+))?$/i;
        var m_relativeRegex = /^((?:(?:[\w\,\.\-\%\+]+)|(?:\/[\w\,\.\-\%\+]+))(?:\/[\w\,\.\-\%\+]*)*)(?:(\#[\w\,\.\-\%\&\;\=\*\!\~\(\)]+)|(\?[\w\,\.\-\%\&\;\=\*\!\~\(\)\/]+))?$/i;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (DataTypeIdentity.IsUndefinedOrNull(originalString)) {
                throw new Error("URI string is undefined.");
            }
            if (!(m_originalString = originalString + String.Empty)) {
                return;
            }
            if (DataTypeIdentity.IsUndefinedOrNull(isRelative)) {
                if (m_isWellFormat = ParseAbsoluteUri()) {
                    m_isRelative = false;
                } else if (m_isWellFormat = ParseRelativeUri()) {
                    m_isRelative = true;
                }
            } else {
                if (m_isRelative = !!isRelative) {
                    m_isWellFormat = ParseRelativeUri();
                } else {
                    m_isWellFormat = ParseAbsoluteUri();
                }
            }
        }

        /*
        * Private Method Description:
        * Parses absolute URI.
        */
        function ParseAbsoluteUri() {
            var match = m_absoluteRegex.exec(m_originalString);
            if (match) {
                m_schema = match[1];
                m_hostname = match[2];
                if (!DataTypeIdentity.IsUndefinedOrNull(match[3]) && match[3].length) {
                    m_port = global.parseInt(match[3]);
                }
                if (!DataTypeIdentity.IsUndefinedOrNull(match[4]) && match[4].length) {
                    m_path = match[4];
                }
                if (!DataTypeIdentity.IsUndefinedOrNull(match[5]) && match[5].length) {
                    m_fragment = match[5];
                }
                if (!DataTypeIdentity.IsUndefinedOrNull(match[6]) && match[6].length) {
                    m_query = match[6];
                }
            }

            return !!match;
        }

        /*
        * Private Method Description:
        * Parses relative URI.
        */
        function ParseRelativeUri() {
            var match = m_relativeRegex.exec(m_originalString);
            if (match) {
                m_path = match[1];
                if (!DataTypeIdentity.IsUndefinedOrNull(match[2]) && match[2].length) {
                    m_fragment = match[2];
                }
                if (!DataTypeIdentity.IsUndefinedOrNull(match[3]) && match[3].length) {
                    m_query = match[3];
                }
            }

            return !!match;
        }

        /*
        * Method Description:
        * Gets the originalString parameter.
        */
        this.GetOriginalString = function () {
            return m_originalString;
        };

        /*
        * Method Description:
        * Gets whether the originalString parameter is a a well format URI string.
        */
        this.GetIsWellFormat = function () {
            return m_isWellFormat;
        };

        /*
        * Method Description:
        * Gets whether the originalString parameter represents a relative URI.
        */
        this.GetIsRelative = function () {
            return m_isRelative;
        };

        /*
        * Method Description:
        * Gets protocol schema of this URI.
        */
        this.GetSchema = function () {
            return m_schema;
        };

        /*
        * Method Description:
        * Gets hostname of this URI.
        */
        this.GetHostname = function () {
            return m_hostname;
        };

        /*
        * Method Description:
        * Gets port of this URI.
        */
        this.GetPort = function () {
            return m_port;
        };

        /*
        * Method Description:
        * Gets hostname and port of this URI, format: hostname:port.
        */
        this.GetHost = function () {
            return (m_hostname || String.Empty) + (m_port ? ":" + m_port : String.Empty);
        };

        /*
        * Method Description:
        * Gets hostname of this URI.
        */
        this.GetPath = function () {
            return m_path;
        };

        /*
        * Method Description:
        * Gets fragment of this URI.
        */
        this.GetFragment = function () {
            return m_path;
        };

        /*
        * Method Description:
        * Gets query string of this URI.
        */
        this.GetQuery = function () {
            return m_query;
        };
    };

    /*
    * Static method Description:
    * Encodes the specified object to a query string.
    *
    * Parameters:
    * obj: a object.
    *
    * Return:
    * The encoded query string.
    */
    Uri.EncodeObjectToQueryString = function (obj) {
        if (!DataTypeIdentity.IsObject(obj)) {
            throw new Error("The object will be encoded is not object type.");
        }
        if (!DataTypeIdentity.IsNotNullObject(obj)) {
            throw new Error("The object will be encoded is null.");
        }

        //encodes the specified name and value pair.
        function EncodePair(name, value) {
            return String.Format("{0}={1}", global.encodeURIComponent(name), global.encodeURIComponent(value + String.Empty));
        }

        //encodes the specified object.
        function Encode(name, obj) {
            switch (typeof obj) {
                case DataTypeNames.String:
                case DataTypeNames.Number:
                case DataTypeNames.Boolean:
                    parts.push(EncodePair(name, obj));
                    break;
                case DataTypeNames.Object:
                    var value = null;
                    if (DataTypeIdentity.IsArray(obj)) {
                        for (var i = 0; i < obj.length; i++) {
                            if (DataTypeIdentity.IsUndefinedOrNull(value = obj[i])) {
                                continue;
                            }

                            Encode(name, value);
                        }
                    } else {
                        for (var property in obj) {
                            if (DataTypeIdentity.IsUndefinedOrNull(value = obj[property])) {
                                continue;
                            }

                            Encode(property, value);
                        }
                    }
                    break;
            }
        }

        var parts = [];
        Encode(null, obj);
        return parts.join("&");
    };

    /*
    * Static method Description:
    * Encodes the specified object to HTTP body.
    *
    * Parameters:
    * obj: a object.
    *
    * Return:
    * The encoded HTTP body string.
    */
    Uri.EncodeObjectToHttpBody = function (obj) {
        return Uri.EncodeObjectToQueryString(obj).replace(/%20/g, "+");
    };

    /***************** BrowserCapability Class Definition *****************/

    /*
    * Static Class Description:
    * Provide functions to detect browser type and ensure browser capability.
    */
    var BrowserCapability = Xphter.BrowserCapability = {
        /*
        * Property Description:
        * Gets a value to indicate whether the browser is Microsfot Internet Explorer.
        */
        /*@cc_on
        @if (@_jscript)
        IsIE: true,
        @else*/
        IsIE: false,
        /*@end
        @*/

        /*
        * Property Description:
        * Gets the browser capability mode of Microsfot Internet Explorer.
        */
        /*@cc_on
        @if (@_jscript)
        IECompatibilityMode: global.document.documentMode || (global.document.compatMode ? (global.document.compatMode == "CSS1Compat" ? 7 : 5) : 5),
        @end
        @*/

        /*
        * Property Description:
        * Gets a value to indicate whether the browser is Opera.
        */
        IsOpera: !!global.opera,

        /*
        * Property Description:
        * Gets a value to indicate whether the browser is Firefox.
        */
        IsFirefox: global.document.mozHidden !== undefined,

        /*
        * Property Description:
        * Gets the major version number of browser.
        */
        /*@cc_on
        @if (@_jscript)
        MajorVersion: global.parseInt(/MSIE (\d+)/i.exec(global.navigator.appVersion)[1])
        @else*/
        MajorVersion: global.parseInt(global.opera ? global.opera.version() + String.Empty : global.navigator.appVersion) || 0
        /*@end
        @*/
    };

    /***************** BrowserGeometry Class Definition *****************/

    /*
    * Static Class Description:
    * Provide geometry information of browser.
    */
    var BrowserGeometry = Xphter.BrowserGeometry = {
        /*
        * Method Description:
        * Gets the X position of the browser window on the screen.
        */
        GetWindowX: function () {
            return !DataTypeIdentity.IsUndefined(global.screenX) ? global.screenX : global.screenLeft;
        },

        /*
        * Method Description:
        * Gets the Y position of the browser window on the screen.
        */
        GetWindowY: function () {
            return !DataTypeIdentity.IsUndefined(global.screenX) ? global.screenY : global.screenTop;
        },

        /*
        * Method Description:
        * Gets the width of the browser window.
        */
        GetWindowWidth: function () {
            return !DataTypeIdentity.IsUndefined(global.outerWidth) ? global.outerWidth : global.document.documentElement ? global.document.documentElement.scrollWidth : global.document.body ? global.document.body.scrollWidth : 0;
        },

        /*
        * Method Description:
        * Gets the height of the browser window.
        */
        GetWindowHeight: function () {
            return !DataTypeIdentity.IsUndefined(global.outerHeight) ? global.outerHeight : global.document.documentElement ? global.document.documentElement.scrollHeight : global.document.body ? global.document.body.scrollHeight : 0;
        },

        /*
        * Method Description:
        * Gets the width of document viewport.
        */
        GetViewportWidth: function () {
            return !DataTypeIdentity.IsUndefined(global.innerWidth) ? global.innerWidth : global.document.documentElement ? global.document.documentElement.clientWidth : global.document.body ? global.document.body.clientWidth : 0;
        },

        /*
        * Method Description:
        * Gets the height of document viewport.
        */
        GetViewportHeight: function () {
            return !DataTypeIdentity.IsUndefined(global.innerHeight) ? global.innerHeight : global.document.documentElement ? global.document.documentElement.clientHeight : global.document.body ? global.document.body.clientHeight : 0;
        },

        /*
        * Method Description:
        * Gets the width of target document viewport.
        */
        GetTargetViewportWidth: function (target) {
            return !DataTypeIdentity.IsUndefined(target.innerWidth) ? target.innerWidth : target.document.documentElement ? target.document.documentElement.clientWidth : target.document.body ? target.document.body.clientWidth : 0;
        },

        /*
        * Method Description:
        * Gets the height of target document viewport.
        */
        GetTargetViewportHeight: function (target) {
            return !DataTypeIdentity.IsUndefined(target.innerHeight) ? target.innerHeight : target.document.documentElement ? target.document.documentElement.clientHeight : target.document.body ? target.document.body.clientHeight : 0;
        },

        /*
        * Method Description:
        * Gets the horizontal scroll offset of the document.
        */
        GetHorizontalScroll: function () {
            return !DataTypeIdentity.IsUndefined(global.pageXOffset) ? global.pageXOffset : global.document.documentElement ? global.document.documentElement.scrollLeft || global.document.body.scrollLeft : global.document.body.scrollLeft;
        },

        /*
        * Method Description:
        * Gets the vertical scroll offset of the document.
        */
        GetVerticalScroll: function () {
            return !DataTypeIdentity.IsUndefined(global.pageYOffset) ? global.pageYOffset : global.document.documentElement ? global.document.documentElement.scrollTop || global.document.body.scrollTop : global.document.body.scrollTop;
        },

        /*
        * Method Description:
        * Sets the horizontal scroll offset of the document.
        */
        SetHorizontalScroll: function (value) {
            if (global.document.documentElement) {
                global.document.documentElement.scrollTop = value;
            }
            if (global.document.body) {
                global.document.body.scrollTop = value;
            }
        },

        /*
        * Method Description:
        * Sets the vertical scroll offset of the document.
        */
        SetVerticalScroll: function (value) {
            if (global.document.documentElement) {
                global.document.documentElement.scrollTop = value;
            }
            if (global.document.body) {
                global.document.body.scrollTop = value;
            }
        },

        /*
        * Method Description:
        * Gets width of document body.
        *
        * Returns:
        * Return the width of document body.
        */
        GetBodyWidth: function () {
            return global.document.body.scrollWidth;
        },

        /*
        * Method Description:
        * Gets height of document body.
        *
        * Returns:
        * Return the height of document body.
        */
        GetBodyHeight: function () {
            return global.document.body.scrollHeight;
        }
    };

    /***************** DocumentUtility Class Definition *****************/

    var elementDefaultDisplayCache = {};

    /*
    * Static Class Description:
    * Provides a utility for operating HTML document.
    */
    var DocumentUtility = Xphter.DocumentUtility = {
        /*
        * Method Description:
        * Append the specified HTML content to current document then new line.
        *
        * Parameters:
        * html: a string represents HTML content.
        */
        WriteLine: function (html) {
            var _html = html + "<br />";
            global.document.write(_html);
        },

        /*
        * Method Description:
        * Determines whether the specified element is a member of the specified CSS class.
        *
        * Parameters:
        * element: a HTML element.
        * className: a CSS class name.
        *
        * Return:
        * Returns true if the specified element is a meber of the specified CSS class, otherwise return false.
        */
        IsCssMember: function (element, className) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is undefined.");
            }

            var _className = className + String.Empty;
            return !_className.IsEmptyOrWhiteSpace() && element.className && new RegExp("(?:^|\\s+)" + RegexEncode(_className) + "(?:$|\\s+)", "gi").test(element.className);
        },

        /*
        * Method Description:
        * Add a CSS class to the specified element.
        *
        * Parameters:
        * element: a HTML element or a HTML element array.
        * className: a CSS class name.
        */
        AddCssClass: function (element, className) {
            function _Add(element, className) {
                if (!DocumentUtility.IsCssMember(element, className)) {
                    if (element.className) {
                        element.className += (String.Space + className);
                    } else {
                        element.className = String.Space + className;
                    }
                }
            }

            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is undefined.");
            }

            var _className = className + String.Empty;
            if (_className.IsEmptyOrWhiteSpace()) {
                throw new Error("CSS class name is undefined.");
            }

            if (DataTypeIdentity.IsArray(element) || ("length" in element) && ("item" in element) && !("type" in element)) {
                var node = null;
                for (var i = 0; i < element.length; i++) {
                    node = element[i];
                    if (DataTypeIdentity.IsNotNullObject(node) && ("nodeType" in node) && node.nodeType == DomNodeTypes.Element) {
                        _Add(node, className);
                    }
                }
            } else {
                _Add(element, className);
            }
        },

        /*
        * Method Description:
        * Remove a CSS class from the specified element.
        *
        * Parameters:
        * element: a HTML element or a HTML element array.
        * className: a CSS class name.
        */
        RemoveCssClass: function (element, className) {
            function _Remove(element, className) {
                DocumentUtility.IsCssMember(element, className) && (element.className = element.className.replace(new RegExp("(?:^|\\s+)" + RegexEncode(_className) + "(?:$|\\s+)", "gi"), String.Empty));
            }

            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is undefined.");
            }

            var _className = className + String.Empty;
            if (_className.IsEmptyOrWhiteSpace()) {
                throw new Error("CSS class name is undefined.");
            }

            if (DataTypeIdentity.IsArray(element) || ("length" in element) && ("item" in element) && !("type" in element)) {
                var node = null;
                for (var i = 0; i < element.length; i++) {
                    node = element[i];
                    if (DataTypeIdentity.IsNotNullObject(node) && ("nodeType" in node) && node.nodeType == DomNodeTypes.Element) {
                        _Remove(node, className);
                    }
                }
            } else {
                _Remove(element, className);
            }
        },

        /*
        * Method Description:
        * Gets elements which are member of the specified CSS class in the descendants of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        * className: a CSS class name.
        *
        * Return:
        * Returns a array of these elements.
        */
        GetElementsByCssClass: function (element, className) {
            var result = [];
            var node = null;
            var elements = [element];
            var _className = className + String.Empty;

            if (!_className.IsEmptyOrWhiteSpace() && DataTypeIdentity.IsNotNullObject(element) && element.childNodes) {
                do {
                    element = elements.shift();

                    for (var i = 0; i < element.childNodes.length; i++) {
                        if ((node = element.childNodes[i]).nodeType != DomNodeTypes.Element) {
                            continue;
                        }

                        if (DocumentUtility.IsCssMember(node, _className)) {
                            result.push(node);
                        }
                        elements.push(node);
                    }
                } while (elements.length > 0);
            }

            return result;
        },

        /*
        * Method Description:
        * Gets elements with the specified tag name in the descendants of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        * tagName: a tag name.
        *
        * Return:
        * Returns a array of these elements.
        */
        GetElementsByTagName: function (element, tagName) {
            var result = [];
            var node = null;
            var elements = [element];
            var _tagName = tagName + String.Empty;

            if (!_tagName.IsEmptyOrWhiteSpace() && DataTypeIdentity.IsNotNullObject(element) && element.childNodes) {
                do {
                    element = elements.shift();

                    for (var i = 0; i < element.childNodes.length; i++) {
                        if ((node = element.childNodes[i]).nodeType != DomNodeTypes.Element) {
                            continue;
                        }

                        if (node.tagName.EqualsIgnoreCase(_tagName)) {
                            result.push(node);
                        }
                        elements.push(node);
                    }
                } while (elements.length > 0);
            }

            return result;
        },

        /*
        * Method Description:
        * Reverses the children of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        */
        ReverseChildren: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The HTML element is undefined.");
            }

            var node = null;
            var fragment = global.document.createDocumentFragment();
            while (node = element.lastChild) {
                fragment.appendChild(element.removeChild(node));
            }
            element.appendChild(fragment);
        },

        /*
        * Method Description:
        * Clears the children of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        */
        ClearChildren: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The HTML element is undefined.");
            }

            var node = null;
            while (node = element.firstChild) {
                element.removeChild(node);
            }
        },

        /*
        * Method Description:
        * Gets the first element child node of the specified HTML element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns the first element child node.
        */
        GetFirstElement: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The HTML element is undefined.");
            }

            var node = element.firstChild;
            while (node) {
                if (node.nodeType === DomNodeTypes.Element) {
                    return node;
                }

                node = node.nextSibling;
            }

            return null;
        },

        /*
        * Method Description:
        * Gets all child elements of the specified HTML element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns all child elements.
        */
        GetChildElements: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The HTML element is undefined.");
            }

            var node = null;
            var result = [];
            var childNodes = element.childNodes;
            for (var i = 0; i < childNodes.length; i++) {
                if ((node = childNodes[i]).nodeType === DomNodeTypes.Element) {
                    result.push(node);
                }
            }

            return result;
        },

        /*
        * Method Description:
        * Gets the child elements of the specified HTML element by the specified tag name.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns the child elements which have the specified tag name.
        */
        GetChildElementsByTagName: function (element, tagName) {
            var _tagName = tagName + String.Empty;
            if (!tagName || _tagName.IsEmptyOrWhiteSpace()) {
                throw new Error("The tag name is undefined.");
            }

            return DocumentUtility.GetChildElements(element).Where(function (item) {
                return item.tagName.EqualsIgnoreCase(_tagName);
            });
        },

        /*
        * Method Description:
        * Create a HTML element with the specified tag.
        *
        * Parameters:
        * tagName: a HTML tag.
        * attribute: a object which enumerable properties represents the properties of the created element.
        * styles: a object which enumerable properties represents the styles of the created element.
        * children: a node array or a NodeList object which elements are child of the created element.
        *
        * Return:
        * Returns the created element.
        */
        CreateElement: function (tagName, attributes, styles, children) {
            var _tagName = tagName + String.Empty;
            if (_tagName.IsEmptyOrWhiteSpace()) {
                throw new Error("The tag name of element is empty.");
            }

            var element = global.document.createElement(_tagName);

            if (DataTypeIdentity.IsNotNullObject(attributes)) {
                var value = null;
                for (var name in attributes) {
                    if (DataTypeIdentity.IsUndefinedOrNull(value = attributes[name])) {
                        continue;
                    }

                    if (name in element) {
                        try {
                            element[name] = value;
                        } catch (ex) {
                            element.setAttribute(name, value);
                        }
                    } else {
                        element.setAttribute(name, value);
                    }
                }
            }

            if (DataTypeIdentity.IsNotNullObject(styles)) {
                var value = null;
                for (var name in styles) {
                    if (DataTypeIdentity.IsUndefinedOrNull(value = styles[name])) {
                        continue;
                    }

                    element.style[name] = value;
                }
            }

            if (DataTypeIdentity.IsArray(children) || (DataTypeIdentity.IsNotNullObject(children) && children.item && children.length)) {
                var child = null;
                for (var i = 0; i < children.length; i++) {
                    if (DataTypeIdentity.IsUndefinedOrNull(child = children[i])) {
                        continue;
                    }

                    if (DataTypeIdentity.IsString(child)) {
                        element.appendChild(global.document.createTextNode(child));
                    } else {
                        element.appendChild(child);
                    }
                }
            } else if (DataTypeIdentity.IsString(children)) {
                element.appendChild(global.document.createTextNode(children));
            } else if (!DataTypeIdentity.IsUndefinedOrNull(children)) {
                element.appendChild(children);
            }

            return element;
        },

        /*
        * Method Description:
        * Create a div HTML element with style: clear both.
        *
        * Return:
        * Returns the created div HTML element.
        */
        CreateClearBoth: function () {
            return DocumentUtility.CreateElement("div", null, {
                clear: "both",
                height: "0px",
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                fontSize: "0px",
                lineHeight: "0px",
                overflow: "hidden"
            });
        },

        /*
        * Method Description:
        * Gets computed style of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns a CSS2Properties object.
        */
        GetComputedStyle: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            if (global.getComputedStyle) {
                try {
                    return global.getComputedStyle(element, null);
                } catch (ex) {
                    return null;
                }
            } else {
                return element.currentStyle || null;
            }
        },

        /*
        * Method Description:
        * Gets the default value of display attribute in CSS by the specified node name.
        *
        * Parameters:
        * nodeName: the node name.
        *
        * Return:
        * Returns the default value of display attribute in CSS.
        */
        GetDefaultDisplay: function (nodeName) {
            function GetActualDisplay(name, doc) {
                var element = doc.createElement(name);
                doc.body.appendChild(element);

                var style = DocumentUtility.GetComputedStyle(element);
                return style ? style.display : null;
            }

            var _nodeName = nodeName + String.Empty;
            if (!_nodeName) {
                throw new Error("The node name is undefined.");
            }

            if (_nodeName in elementDefaultDisplayCache) {
                return elementDefaultDisplayCache[_nodeName];
            }

            var doc = global.document;
            var display = GetActualDisplay(_nodeName, doc);

            if (display == "none" || !display) {
                var iframe = global.document.createElement("<iframe frameborder='0' width='0' height='0'>");
                global.document.body.appendChild(iframe);

                doc = iframe.contentWindow.document;
                doc.write();
                doc.close();

                display = GetActualDisplay(_nodeName, doc);
                global.document.body.removeChild(iframe);
            }

            return elementDefaultDisplayCache[_nodeName] = display;
        },

        /*
        * Method Description:
        * Shows the specified HTML element.
        *
        * Parameters:
        * element: a HTML element.
        */
        ShowElement: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is undefined.");
            }
            if (!("tagName" in element)) {
                throw new Error("element is not a HTML element.");
            }

            element.style.display = DocumentUtility.GetDefaultDisplay(element.tagName);
        },

        /*
        * Method Description:
        * Hides the specified HTML element.
        *
        * Parameters:
        * element: a HTML element.
        */
        HideElement: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is undefined.");
            }
            if (!("tagName" in element)) {
                throw new Error("element is not a HTML element.");
            }

            element.style.display = "none";
        },

        /*
        * Method Description:
        * Gets absolute width of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        * isInner: whether to get inner width.
        *
        * Return:
        * Returns absolute width of the specified element.
        */
        GetElementWidth: function (element, isInner) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            css = DocumentUtility.GetComputedStyle(element);
            if (css) {
                if (!!isInner) {
                    return element.offsetWidth - (global.parseInt(css.paddingLeft) || 0) - (global.parseInt(css.paddingRight) || 0);
                } else {
                    return element.offsetWidth + (global.parseInt(css.marginLeft) || 0) + (global.parseInt(css.marginRight) || 0);
                }
            } else {
                return element.offsetWidth;
            }
        },

        /*
        * Method Description:
        * Gets absolute height of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        * isInner: whether to get inner height.
        *
        * Return:
        * Returns absolute height of the specified element.
        */
        GetElementHeight: function (element, isInner) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            var css = DocumentUtility.GetComputedStyle(element);

            if (css) {
                if (!!isInner) {
                    return element.offsetHeight - (global.parseInt(css.paddingTop) || 0) - (global.parseInt(css.paddingBottom) || 0);
                } else {
                    var marginTop = (global.parseInt(css.marginTop) || 0);
                    var previousSibling = element.previousSibling;
                    var previousCss = previousSibling ? DocumentUtility.GetComputedStyle(previousSibling) : null;
                    previousCss && (marginTop = Math.max(marginTop, global.parseInt(previousCss.marginBottom) || 0));

                    var marginBottom = (global.parseInt(css.marginBottom) || 0);
                    var nextSibling = element.nextSibling;
                    var nextCss = nextSibling ? DocumentUtility.GetComputedStyle(nextSibling) : null;
                    nextCss && (marginBottom = Math.max(marginBottom, global.parseInt(nextCss.marginTop) || 0));

                    return element.offsetHeight + marginTop + marginBottom;
                }
            } else {
                return element.offsetHeight;
            }
        },

        /*
        * Method Description:
        * Gets absolute X position of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns absolute X position of the specified element.
        */
        GetElementX: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            var x = 0;
            var style = DocumentUtility.GetComputedStyle(element);
            var isFixed = style && style.position && style.position.EqualsIgnoreCase("fixed");
            for (var e = element; e; e = e.offsetParent) {
                x += e.offsetLeft;
            }
            if (!isFixed) {
                for (var e = element.parentNode; e && e != global.document.body && e != global.document.documentElement; e = e.parentNode) {
                    e.scrollLeft && (x -= e.scrollLeft);
                }
                if (global.document.body.scrollLeft) {
                    x -= global.document.body.scrollLeft;
                } else if (global.document.documentElement.scrollLeft) {
                    x -= global.document.documentElement.scrollLeft;
                }
            }

            return x;
        },

        /*
        * Method Description:
        * Gets absolute Y position of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns absolute Y position of the specified element.
        */
        GetElementY: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            var y = 0;
            var style = DocumentUtility.GetComputedStyle(element);
            var isFixed = style && style.position && style.position.EqualsIgnoreCase("fixed");
            for (var e = element; e; e = e.offsetParent) {
                y += e.offsetTop;
            }
            if (!isFixed) {
                for (var e = element.parentNode; e && e != global.document.body && e != global.document.documentElement; e = e.parentNode) {
                    e.scrollTop && (y -= e.scrollTop);
                }
                if (global.document.body.scrollTop) {
                    y -= global.document.body.scrollTop;
                } else if (global.document.documentElement.scrollTop) {
                    y -= global.document.documentElement.scrollTop;
                }
            }

            return y;
        },

        /*
        * Method Description:
        * Gets the outer HTML of the specified element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns outer HTML of the specified element.
        */
        GetOuterHTML: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                return null;
            }

            var parent = element.parentNode;
            var nextSibling = element.nextSibling;

            var wrap = global.document.createElement("div");
            wrap.appendChild(element);
            var outerHTML = wrap.innerHTML;

            parent && parent.insertBefore(element, nextSibling);

            return outerHTML;
        },

        /*
        * Method Description:
        * Sets the inner HTML of the specified element.
        * The inner HTML can contains scripts.
        *
        * Parameters:
        * element: a HTML element.
        * innerHTML: the inner HTML.
        */
        SetInnerHTML: function (element, innerHTML) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is null or not a object.");
            }
            if (!("innerHTML" in element)) {
                throw new Error("element do not has a property named innerHTML");
            }

            if (!(innerHTML = innerHTML + String.Empty)) {
                return;
            }

            // IE6/7/8/9 can run the scripts(with defer attribute) in innerHTML
            function FilterDefer(content) {
                // search defer attributes
                var deferResult = null;
                var deferPositions = [];
                var deferRegex = /\s+defer(?:\s*=\s*(?:\w+|'[^']*'|"[^"]*"))?/ig;
                while ((deferResult = deferRegex.exec(content)) != null) {
                    deferPositions.push({
                        index: deferResult.index,
                        length: deferResult[0].length
                    });
                }
                if (!deferPositions.length) {
                    return content;
                }

                // search script start tags
                var scriptResult = null;
                var scriptPositions = [];
                var scriptRegex = /<script[^<>]*>/ig;
                while ((scriptResult = scriptRegex.exec(content)) != null) {
                    scriptPositions.push({
                        index: scriptResult.index,
                        length: scriptResult[0].length
                    });
                }
                if (!scriptPositions.length) {
                    return content;
                }

                // search defer attributes in script open tags.
                deferPositions = deferPositions.Where(function (item) {
                    return scriptPositions.Any(function (obj) {
                        return obj.index < item.index && obj.index + obj.length > item.index + item.length;
                    });
                });
                if (!deferPositions.length) {
                    return content;
                }

                // remove defer attributes;
                var fragments = [], deferPosition = null, startIndex = 0;
                for (var i = 0; i < deferPositions.length; i++) {
                    deferPosition = deferPositions[i];
                    fragments.push(content.slice(startIndex, deferPosition.index));
                    startIndex = deferPosition.index + deferPosition.length;
                }
                if (startIndex < content.length) {
                    fragments.push(content.slice(startIndex));
                }

                return fragments.join(String.Space);
            }

            function FindNextSilbing(target) {
                var silbing = target.nextSibling;

                while (silbing != null && "script".EqualsIgnoreCase(silbing.nodeName)) {
                    silbing = silbing.nextSibling;
                }

                return silbing;
            }

            function ImportScript(index) {
                if (index >= importData.length) {
                    return;
                }

                var source = importData[index].script;
                var parentNode = importData[index].parentNode;
                var nextSibling = importData[index].nextSibling;

                var isAsync = source.hasAttribute("async") ? source.async : false;
                var script = DocumentUtility.CreateElement("script", {
                    type: source.type || "text/javascript",
                    src: source.src || null,
                    async: isAsync,
                    charset: source.charset || null,
                    language: source.language || null,
                    text: source.text || null
                });
                if (!isAsync) {
                    if (script.src) {
                        EventUtility.RegisterScriptLoad(script, function () {
                            ImportScript(index + 1);
                        });
                    } else {
                        global.setTimeout(function () {
                            ImportScript(index + 1);
                        }, 0);
                    }
                }

                parentNode.insertBefore(script, nextSibling);

                if (isAsync) {
                    ImportScript(index + 1);
                }
            }

            innerHTML = FilterDefer(innerHTML);
            element.innerHTML = innerHTML;
            var scripts = element.getElementsByTagName("script");
            if (!scripts.length) {
                return;
            }

            var importData = [];
            for (var i = 0; i < scripts.length; i++) {
                importData.push({
                    script: scripts[i],
                    parentNode: scripts[i].parentNode,
                    nextSibling: FindNextSilbing(scripts[i])
                });
            }
            while (scripts.length) {
                scripts[0].parentNode.removeChild(scripts[0]);
            }
            ImportScript(0);
        },

        /*
        * Method Description:
        * Determines whether node1 is a child of node2.
        *
        * Parameters:
        * node1: a HTML node.
        * node2: a HTML node.
        *
        * Return:
        * Returns true if node1 is a child of node2, otherwise false.
        */
        IsChild: function (node1, node2) {
            if (!node1 || !node2 || node2.parentNode == node1) {
                return false;
            }

            var result = false;
            var parent1 = node1, parent2 = node2.parentNode;

            while (parent1 = parent1.parentNode) {
                if (parent1 == node2) {
                    result = true;
                    break;
                }
                if (parent1 == parent2) {
                    break;
                }
            }

            return result;
        },

        /*
        * Method Description:
        * Determines whether the specified element is a global window object.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns true if the specified element is a global window object, otherwise return false.
        */
        IsWindow: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is not a avaliable object.");
            }

            var result = false;
            if ("window" in element) {
                var backup = element.window;
                try {
                    if (!(delete element.window)) {
                        result = element.window === element.window.window;
                    } else {
                        element.window = backup;
                    }
                } catch (ex) {
                    result = element.window === element.window.window;
                }
            }

            return result;
        },

        /*
        * Method Description:
        * Gets the window object which contains the specified element.
        *
        * Parameters:
        * element: a HTML element.
        *
        * Return:
        * Returns the window object which contains the specified element.
        */
        GetWindowObject: function (element) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("element is not a avaliable object.");
            }
            if (DocumentUtility.IsWindow(element)) {
                return element;
            }

            var doc = element.ownerDocument || element;
            return doc.defaultView || doc.parentWindow;
        },

        /*
        * Method Description:
        * Encodes the specified form variables to a query string.
        *
        * Parameters:
        * form: a HTML Form element.
        *
        * Return:
        * The encoded query string.
        */
        EncodeFormToQueryString: function (form) {
            if (!DataTypeIdentity.IsNotNullObject(form)) {
                throw new Error("form is undefined.");
            }
            if (!("elements" in form)) {
                throw new Error("form is not a HTML Form element");
            }

            var data = {};
            var element = null, option = null;
            for (var i = 0; i < form.elements.length; i++) {
                element = form.elements[i];
                if (!("type" in element) || !element.name) {
                    continue;
                }

                if (!(element.name in data)) {
                    data[element.name] = [];
                }
                switch (element.type.toLowerCase()) {
                    case "button":
                    case "reset":
                        //ignore types
                        break;
                    case "text":
                    case "textarea":
                    case "hidden":
                    case "password":
                    case "email":
                    case "url":
                    case "number":
                    case "search":
                    case "color":
                    case "range":
                    case "date":
                    case "month":
                    case "week":
                    case "time":
                    case "datetime":
                    case "datetime-local":
                        data[element.name].push(element.value);
                        break;
                    case "checkbox":
                    case "radio":
                        if (element.checked) {
                            data[element.name].push(element.value);
                        }
                        break;
                    case "submit":
                        if (element.value) {
                            data[element.name].push(element.value);
                        }
                        break;
                    case "select-one":
                    case "select-multiple":
                        for (var j = 0; j < element.options.length; j++) {
                            if (!(option = element.options[j]).selected) {
                                continue;
                            }

                            data[element.name].push(option.value);
                        }
                        break;
                }
            }

            return Uri.EncodeObjectToQueryString(data);
        },

        /*
        * Method Description:
        * Encodes the specified form variables to HTTP body.
        *
        * Parameters:
        * form: a HTML Form element.
        *
        * Return:
        * The encoded HTTP body string.
        */
        EncodeFormToHttpBody: function (form) {
            return DocumentUtility.EncodeFormToQueryString(form).replace(/%20/g, "+");
        },

        /*
        * Method Description:
        * Gets the selected value of a SELECT element.
        *
        * Parameters:
        * list: a HTML SELECT element.
        */
        GetDropdownSelectedValue: function (list) {
            if (!list) {
                throw new Error("Dropdown list is undefined.");
            }

            return list.selectedIndex >= 0 ? list.options[list.selectedIndex].value : null;
        },

        /*
        * Method Description:
        * Gets the selected text of a SELECT element.
        *
        * Parameters:
        * list: a HTML SELECT element.
        */
        GetDropdownSelectedText: function (list) {
            if (!list) {
                throw new Error("Dropdown list is undefined.");
            }

            return list.selectedIndex >= 0 ? list.options[list.selectedIndex].text : null;
        },

        /*
        * Method Description:
        * Sets selected value to the specified value of a SELECT element.
        *
        * Parameters:
        * list: a HTML SELECT element.
        * value: the selected value.
        */
        SetDropdownSelectedValue: function (list, value) {
            if (!list) {
                throw new Error("Dropdown list is undefined.");
            }
            if (DataTypeIdentity.IsUndefinedOrNull(value)) {
                list.selectedIndex = -1;
                return;
            }

            var option = null;
            var _value = value + String.Empty;
            for (var i = 0; i < list.options.length; i++) {
                option = list.options[i];
                option.selected = option.value.EqualsIgnoreCase(_value);
            }
        },

        /*
        * Method Description:
        * Registers a event handler for text changed event of the specified text element.
        * (compatible for old code.)
        *
        * Parameters:
        * textElement: a HTML TEXT element(input type="text", textarea).
        * handler: The event handler.
        */
        OnTextChanged: function (textElement, handler) {
            EventUtility.RegisterTextChanged(textElement, handler);
        },

        /*
        * Method Description:
        * Updates the height of a iframe element.
        *
        * Parameters:
        * frame: a HTML IFRAME element.
        */
        UpdateFrameHeight: function (frame) {
            if (!frame) {
                throw new Error("frame is null or undefined");
            }

            var frameDocument = frame.contentWindow.document;
            frameDocument && (frameDocument.documentElement || frameDocument.body) && (frame.height = (frameDocument.documentElement || frameDocument.body).scrollHeight + "px");
        }
    };

    /***************** TextArea Class Definition *****************/

    /*
    * Class Description:
    * Represents a cross-browser TextArea tool.
    *
    * Constructor Parameters:
    * textarea : The HTML textarea element;
    */
    var TextArea = Xphter.TextArea = function (textarea) {
        var m_textarea = null;
        var m_newLine = null;
        var m_selection = {
            StartIndex: 0,
            EndIndex: 0,
            GetText: function () {
                if (this.StartIndex >= this.EndIndex) {
                    return String.Empty;
                } else {
                    return m_textarea.value.substring(this.StartIndex, this.EndIndex);
                }
            }
        };

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(textarea)) {
                throw new Error("The textarea element is undefined.");
            }
            if (!textarea.type || !"textarea".EqualsIgnoreCase(textarea.type)) {
                throw new Error("textarea is not a HTML textarea element.");
            }

            m_textarea = textarea;
            m_newLine = m_textarea.setSelectionRange ? "\n" : "\r\n";
            EventUtility.Register(m_textarea, "keyup", UpdateSelection, false);
            EventUtility.Register(m_textarea, "click", UpdateSelection, false);
        }

        //Update current selection.
        function UpdateSelection() {
            var startIndex = 0;
            var endIndex = 0;

            if (m_textarea.setSelectionRange) {
                startIndex = m_textarea.selectionStart;
                endIndex = m_textarea.selectionEnd;
            } else {
                var selectedRange = global.document.selection.createRange();
                var length = selectedRange.text.length;
                var textRange = global.document.body.createTextRange();
                textRange.moveToElementText(m_textarea);

                var text = m_textarea.value;
                while (textRange.compareEndPoints("StartToStart", selectedRange) < 0) {
                    selectedRange.moveStart("character", -1);
                    if (text.charAt(startIndex) === "\r") {
                        ++startIndex;
                    }
                    ++startIndex;
                }
                endIndex = startIndex + length;
            }

            m_selection.StartIndex = startIndex;
            m_selection.EndIndex = endIndex;
        }

        //Correct the speicified string
        function Normalize(text) {
            if (text.length == 0) {
                return text;
            }

            var result = String.Empty;

            if (m_textarea.setSelectionRange) {
                result = text.replace(/\r\n/gi, "\n");
            } else {
                var regex = /\n/gi;
                var lastIndex = 0;
                var match = null;
                while (match = regex.exec(text)) {
                    if (match.index > lastIndex) {
                        if (text.charAt(match.index - 1) === "\r") {
                            result += text.substring(lastIndex, match.lastIndex);
                        } else {
                            result += text.substring(lastIndex, match.index) + "\r\n";
                        }
                    } else {
                        result += "\r\n";
                    }
                    lastIndex = match.lastIndex;
                }
            }

            return result;
        }

        /*
        * Method Description:
        * Gets current selection.
        */
        this.GetSelection = function () {
            return m_selection;
        };

        /*
        * Method Description:
        * Sets current selection to the specified position.
        */
        this.SetSelection = function (startIndex, endIndex) {
            var _startIndex = startIndex - 0;
            var _endIndex = endIndex - 0;
            if (global.isNaN(_startIndex) || global.isNaN(_endIndex)) {
                throw new Error("Start index or end index must be a number.");
            }
            if (_startIndex < 0 || _endIndex < 0) {
                throw new Error("Start index or end index is less than zero.");
            }
            if (_startIndex > m_textarea.value.length || _endIndex > m_textarea.value.length) {
                throw new Error("Start index or end index is points out of range");
            }

            if (_startIndex > _endIndex) {
                _startIndex += _endIndex;
                _endIndex = _startIndex - _endIndex;
                _startIndex = _startIndex - _endIndex;
            }
            if (_startIndex == _endIndex) {
                if (_startIndex == 0) {
                    this.SetCaretToStart();
                    return;
                }
                if (_startIndex == m_textarea.length) {
                    this.SetCaretToEnd();
                    return;
                }
            }

            m_textarea.focus();
            if (m_textarea.setSelectionRange) {
                m_textarea.setSelectionRange(_startIndex, _endIndex);
            } else {
                var text = m_textarea.value;
                if (text.charAt(_startIndex) === "\n") {
                    ++_startIndex;
                }
                if (text.charAt(_endIndex) === "\n") {
                    ++_endIndex;
                }

                var startIndexOffset = 0;
                for (var i = 0; i < _startIndex; i++) {
                    if (text.charAt(i) === "\r") {
                        --startIndexOffset;
                    }
                }

                var endIndexOffset = 0;
                for (var i = _startIndex; i < _endIndex; i++) {
                    if (text.charAt(i) === "\r") {
                        --endIndexOffset;
                    }
                }

                var range = global.document.body.createTextRange();
                range.moveToElementText(m_textarea);
                _startIndex && range.moveStart("character", _startIndex + startIndexOffset);
                range.collapse(true);
                _endIndex - _startIndex > 0 && range.moveEnd("character", _endIndex - _startIndex + endIndexOffset);
                range.select();
            }

            m_selection.StartIndex = _startIndex;
            m_selection.EndIndex = _endIndex;
        };

        /*
        * Method Description:
        * Sets the caret to start of text content.
        */
        this.SetCaretToStart = function () {
            var index = 0;

            m_textarea.focus();
            if (m_textarea.setSelectionRange) {
                m_textarea.setSelectionRange(index, index);
            } else {
                var range = global.document.body.createTextRange();
                range.moveToElementText(m_textarea);
                range.collapse(true);
                range.select();
            }

            m_selection.StartIndex = m_selection.EndIndex = index;
        };

        /*
        * Method Description:
        * Sets the caret to end of text content.
        */
        this.SetCaretToEnd = function () {
            var index = m_textarea.value.length;

            m_textarea.focus();
            if (m_textarea.setSelectionRange) {
                m_textarea.setSelectionRange(index, index);
            } else {
                var range = global.document.body.createTextRange();
                range.moveToElementText(m_textarea);
                range.collapse(false);
                range.select();
            }

            m_selection.StartIndex = m_selection.EndIndex = index;
        };

        /*
        * Method Description:
        * Replaces current selection with the specified string.
        *
        * Return:
        * Returns the actual replacement string.
        *
        * Remark:
        * If the selection length is zero, then insert the specified string to the caret position.
        */
        this.ReplaceSelection = function (replacement) {
            var _replacement = String.Empty;
            if (!DataTypeIdentity.IsUndefinedOrNull(replacement)) {
                _replacement = replacement + String.Empty;
            }
            _replacement = Normalize(_replacement);

            var text = m_textarea.value;
            m_textarea.value = text.substring(0, m_selection.StartIndex) + _replacement + text.substring(m_selection.EndIndex, text.length);

            this.SetSelection(m_selection.StartIndex, m_selection.StartIndex + _replacement.length);

            return _replacement;
        };

        /*
        * Method Description:
        * Gets the text content.
        */
        this.GetText = function () {
            return m_textarea.value;
        };

        /*
        * Method Description:
        * Sets the text content.
        */
        this.SetText = function (text) {
            var _text = String.Empty;
            if (!DataTypeIdentity.IsUndefinedOrNull(text)) {
                _text = text + String.Empty;
            }
            m_textarea.value = _text;
        };

        /*
        * Method Description:
        * Gets the string represents a new line.
        */
        this.GetNewLine = function () {
            return m_newLine;
        };
    }

    /***************** Cookie Class Definition *****************/

    /*
    * Class Description:
    * Represents a cookie.
    *
    * Constructor Parameters:
    * name : Cookie name;
    * value: Cookie value.
    * maxAge: The persist seconds.
    * path: The associated range path.
    * domain: Cookie domain.
    * isEncrypted: Whether encrypt this cookie.
    */
    var Cookie = Xphter.Cookie = function (name, value, maxAge, path, domain, isEncrypted) {
        var m_name = null;
        var m_value = null;
        var m_maxAge = null;
        var m_path = null;
        var m_domain = null;
        var m_isEncrypted = null;

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            if (!name) {
                throw new Error("cookie name is undefined.");
            }
            if (!Cookie.IsValidName(m_name = name + String.Empty)) {
                throw new Error("cookie name contains invalid character.");
            }
            m_value = value + String.Empty;
            !global.isNaN(maxAge) && (m_maxAge = maxAge - 0);
            path && (m_path = path + String.Empty);
            domain && (m_domain = domain + String.Empty);
            !DataTypeIdentity.IsUndefinedOrNull(isEncrypted) && (m_isEncrypted = !!isEncrypted);
        }

        /*
        * Method Description:
        * Gets name of this cookie.
        */
        this.GetName = function () {
            return m_name;
        };

        /*
        * Method Description:
        * Gets value of this cookie.
        */
        this.GetValue = function () {
            return m_value;
        };

        /*
        * Method Description:
        * Gets max-age of this cookie.
        */
        this.GetMaxAge = function () {
            return m_maxAge;
        };

        /*
        * Method Description:
        * Gets path of this cookie.
        */
        this.GetPath = function () {
            return m_path;
        };

        /*
        * Method Description:
        * Gets domain of this cookie.
        */
        this.GetDomain = function () {
            return m_domain;
        };

        /*
        * Method Description:
        * Gets whether encrypt this cookie.
        */
        this.GetPath = function () {
            return !!m_isEncrypted;
        };

        /*
        * Method Description:
        * Gets string value of this cookie.
        */
        this.GetCookieString = function () {
            return m_name + "=" + global.encodeURIComponent(m_value) + (m_maxAge === null ? String.Empty : "; max-age=" + m_maxAge) + (m_path === null ? String.Empty : "; path=" + m_path) + (m_domain === null ? String.Empty : "; domain=" + m_domain) + (m_isEncrypted ? "; secure" : String.Empty);
        };
    };

    /*
    * Static Method Description:
    * Determine whether the specified name is a valid cookie name.
    * 
    * Parameters:
    * name: The name which will be check.
    */
    Cookie.IsValidName = function (name) {
        if (!DataTypeIdentity.IsString(name)) {
            return false;
        }

        var isValid = true;
        var character = null;
        for (var i = 0; i < name.length; i++) {
            character = name.charAt(i);
            if (character == "=" || character == ";" || character == "," || character == "\r" || character == "\n" || character == "\t" || character == " ") {
                isValid = false;
                break;
            }
        }

        return isValid;
    };

    /***************** Cookies Class Definition *****************/

    /*
    * Class Description:
    * Provides a utility for operate cookies, such as add, update, delete, query.
    */
    var Cookies = Xphter.Cookies = (function () {
        //singleton pattern: class implementation
        var Implementation = function () {
            var m_cookies = [];

            /*
            * Method Description:
            * Refresh cookie cache.
            */
            this.Refresh = function () {
                for (var i = 0; i < m_cookies.length; i++) {
                    delete this[m_cookies[i].GetName()];
                }
                m_cookies = [];

                var name = null;
                var value = null;
                var result = null;
                var regex = /([^\=\;\,\s]+)\=([^\;\,\s]+)(?:\;|$)/g;
                var cookieValue = global.document.cookie;
                while ((result = regex.exec(cookieValue)) != null) {
                    name = result[1];
                    try {
                        value = global.decodeURIComponent(result[2]);
                    } catch (ex) {
                        value = result[2];
                    }

                    this[name] = value;
                    m_cookies.push(new Cookie(name, value));
                }
            };

            /*
            * Method Description:
            * Determine whether the cookie will the specified name is existing.
            * 
            * Parameters:
            * name: Cookie name.
            */
            this.IsExists = function (name) {
                var existing = false;
                var _name = name + String.Empty;
                for (var i = 0; i < m_cookies.length; i++) {
                    if (m_cookies[i].GetName() == _name) {
                        existing = true;
                        break;
                    }
                }
                return existing;
            };

            /*
            * Method Description:
            * Gets the cookie with the specified name.
            * 
            * Parameters:
            * name: Cookie name.
            *
            * Returns:
            * Return a Cookie object or null if it is not existing.
            */
            this.GetCookie = function (name) {
                var cookie = null;
                var _name = name + String.Empty;
                for (var i = 0; i < m_cookies.length; i++) {
                    if (m_cookies[i].GetName() == _name) {
                        cookie = m_cookies[i];
                        break;
                    }
                }
                return cookie;
            };

            /*
            * Method Description:
            * Gets the all cookies.
            *
            * Returns:
            * Return a array of Cookie object.
            */
            this.GetCookies = function () {
                var cookies = [];
                for (var i = 0; i < m_cookies.length; i++) {
                    cookies.push(m_cookies[i]);
                }
                return cookies;
            };

            /*
            * Method Description:
            * Add a new cookie. If it is already existing, then the existing cookie will be update.
            * 
            * Parameters:
            * cookie: A Cookie object.
            *
            * Returns:
            * Return the added Cookie object.
            */
            this.Add = function (cookie) {
                if (DataTypeIdentity.IsNotNullObject(cookie) && cookie instanceof Cookie) {
                    global.document.cookie = cookie.GetCookieString();
                    this.Refresh();
                }
                return cookie;
            };

            /*
            * Method Description:
            * Delete the specified cookie.
            * 
            * Parameters:
            * cookie: A Cookie object.
            */
            this.Delete = function (cookie) {
                if (DataTypeIdentity.IsNotNullObject(cookie) && cookie instanceof Cookie) {
                    this.Add(new Cookie(cookie.GetName(), String.Empty, 0, cookie.GetPath(), cookie.GetDomain(), null));
                    this.Refresh();
                }
            };
        };

        //singleton instance
        var m_instance = null;
        return {
            GetInstance: function () {
                if (!m_instance) {
                    //create instance
                    m_instance = new Implementation();
                    m_instance.Refresh();
                }

                return m_instance;
            }
        };
    })();

    /***************** QueryString Class Definition *****************/

    /*
    * Class Description:
    * Analyze the query string:
    * set key as a property of this class;
    * set value as the value of this property;
    */
    var QueryString = Xphter.QueryString = (function () {
        //singleton pattern: class implementation
        var Implementation = function (search) {
            if (search) {
                search.StartsWith("?") && (search = search.slice(1));

                var match = null;
                var regex = /([^\=\&\s]+)\=([^\=\&\s]+)(?:\&|$)/g;
                while (match = regex.exec(search.toLowerCase())) {
                    try {
                        this[match[1]] = global.decodeURIComponent(match[2]);
                    } catch (ex) {
                        this[match[1]] = match[2];
                    }
                }
            }
        };

        //singleton instance
        var m_globalInstance = null;
        var m_currentInstances = [];
        return {
            /*
            * Static Method Description:
            * Gets the unique instance of global runtime environment.
            * 
            * Parameters:
            * name: A dictionay object.
            */
            GetGlobalInstance: function () {
                if (!m_globalInstance) {
                    //create instance
                    m_globalInstance = new Implementation(global.location.search);
                }

                return m_globalInstance;
            },

            /*
            * Static Method Description:
            * Gets the unique instance of current executing environment.
            * 
            * Parameters:
            * name: A dictionay object.
            */
            GetCurrentInstance: function () {
                var index = 0;
                var query = null;
                var instance = {};
                var element = null;
                var elements = global.document.getElementsByTagName("script");
                if (elements.length && (element = elements[index = elements.length - 1]) && element.src) {
                    if (!(instance = m_currentInstances[index])) {
                        var uri = new Uri(element.src);
                        if (uri.GetIsWellFormat() && (query = uri.GetQuery())) {
                            instance = m_currentInstances[index] = new Implementation(query);
                        } else {
                            instance = m_currentInstances[index] = {};
                        }
                    }
                }
                return instance;
            },

            /*
            * Static Method Description:
            * Creates a new instance from the specified query string.
            * 
            * Parameters:
            * name: A dictionay object.
            */
            Parse: function (search) {
                return new Implementation(search);
            }
        };
    })();

    /***************** EventUtility Class Definition *****************/

    /*
    * Class Description:
    * Represents a event layer for IE.
    */
    var IEEventLayer = (function () {
        //singleton pattern: class implementation
        var Implementation = function () {
            //all registered handler array.
            this.Handlers = [];

            //Gets the index of existing event handler in element events cache. If it is not existing, return -1.
            this.GetIndex = function (element, event, handler, useCapture) {
                var index = -1;
                var entity = null;

                if (element._ie_handlers) {
                    for (var i = 0; i < element._ie_handlers.length; i++) {
                        if (!(entity = this.Handlers[element._ie_handlers[i]])) {
                            continue;
                        }

                        if (entity.Element == element && entity.Event == event && entity.Handler == handler && entity.UseCapture == useCapture) {
                            index = i;
                            break;
                        }
                    }
                }

                return index;
            }

            //register a event handler for element in IE.
            this.Register = function (element, event, handler, useCapture) {
                //break duplicate register
                if (this.GetIndex(element, event, handler, useCapture) >= 0) {
                    return;
                }

                //create the real event handler.
                var realHandler = function () {
                    var e = DocumentUtility.GetWindowObject(element).event;
                    handler.call(element, {
                        type: event,
                        target: e.srcElement,
                        currentTarget: element,
                        bubbles: e.srcElement != element,
                        cancelable: true,
                        detail: null,
                        wheelDelta: e.wheelDelta,
                        button: e.button,
                        altKey: e.altKey,
                        ctrlKey: e.ctrlKey,
                        metaKey: null,
                        shiftKey: e.shiftKey,
                        clientX: e.clientX,
                        clientY: e.clientY,
                        screenX: e.screenX,
                        screenY: e.screenY,
                        relatedTarget: event == "mouseover" ? e.fromElement : (event == "mouseout" ? e.toElement : null),
                        charCode: e.keyCode,
                        keyCode: e.keyCode,
                        stopPropagation: function () {
                            e.cancelBubble = true;
                        },
                        preventDefault: function () {
                            e.returnValue = false;
                        }
                    });
                };

                //register event handler
                element.attachEvent("on" + event, realHandler);

                //store info of the event handler.
                this.Handlers.push({
                    Element: element,
                    Event: event,
                    Handler: handler,
                    UseCapture: useCapture,
                    RealHandler: realHandler
                });

                !element._ie_handlers && (element._ie_handlers = []);
                element._ie_handlers.push(this.Handlers.length - 1);
            };

            //unregister a event handler for element in IE.
            this.Unregister = function (element, event, handler, useCapture) {
                var index = this.GetIndex(element, event, handler, useCapture);
                if (index >= 0) {
                    element.detachEvent("on" + event, this.Handlers[element._ie_handlers[index]].RealHandler);

                    this.Handlers[element._ie_handlers[index]] = null;
                    element._ie_handlers.splice(index, 1);
                }
            };

            //unregister all event handlers.
            this.UnregisterAll = function () {
                var entity = null;
                for (var i = 0; i < this.Handlers.length; i++) {
                    if (!(entity = this.Handlers[i])) {
                        continue;
                    }

                    entity.Element.detachEvent("on" + entity.Event, entity.RealHandler);
                    this.Handlers[i] = null;
                }
            }
        };

        //singleton instance
        var m_instance = null;
        return {
            GetInstance: function () {
                if (!m_instance) {
                    //create instance
                    m_instance = new Implementation();

                    //unregister all event handlers when page unloading.
                    global.attachEvent("onunload", function () {
                        m_instance.UnregisterAll();
                    });
                }

                return m_instance;
            }
        };
    })();

    /*
    * Static Class Description:
    * Provides a utility for register and unregister element event handler.
    */
    var EventUtility = Xphter.EventUtility = {
        /*
        * Static Method Description:
        * Register a event handler for the specified element.
        * 
        * Parameters:
        * element: The element will be registered a event handler.
        * event: The event type without "on" prefix.
        * handler: The event handler.
        * useCapture: Whether register a capture handler.
        */
        Register: function (element, event, handler, useCapture) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The element which will register event handler is undefined");
            }
            if (!event) {
                throw new Error("The event type is undefined");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The event handler is undefiend");
            }

            if (element.addEventListener) {
                element.addEventListener(event, handler, useCapture);
            } else {
                IEEventLayer.GetInstance().Register(element, event, handler, useCapture);
            }
        },

        /*
        * Static Method Description:
        * Register a event handler of global window when the user presses the ENTER key.
        *
        * Parameters:
        * handler: The event handler.
        */
        RegisterEnter: function (handler) {
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The event handler is undefiend");
            }

            if (global.addEventListener) {
                global.addEventListener("keydown", function (event) {
                    if (event.target.tagName && event.target.tagName.EqualsIgnoreCase("textarea")) {
                        return;
                    }
                    if (event.keyCode != KeyCodes.NewLine && event.keyCode != KeyCodes.Return) {
                        return;
                    }

                    try {
                        handler(event);
                    } finally {
                        event.preventDefault();
                    }
                }, false);
            } else {
                /*
                * attention: the keydown event can not bubbles to window object in IE6/7/8!
                */
                IEEventLayer.GetInstance().Register(global.document, "keydown", function (event) {
                    if (event.target.tagName && event.target.tagName.EqualsIgnoreCase("textarea")) {
                        return;
                    }
                    if (event.keyCode != KeyCodes.NewLine && event.keyCode != KeyCodes.Return) {
                        return;
                    }

                    try {
                        handler(event);
                    } finally {
                        event.preventDefault();
                    }
                }, false);
            }
        },

        /*
        * Static Method Description:
        * Register a event handler for mouse wheel event of the specified element.
        * 
        * Parameters:
        * element: The element will be registered a event handler.
        * handler: The event handler.
        * useCapture: Whether register a capture handler.
        */
        RegisterMouseWheel: function (element, handler, useCapture) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The element which will register event handler is undefined");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The event handler is undefiend");
            }

            EventUtility.Register(element, BrowserCapability.IsFirefox ? "DOMMouseScroll" : "mousewheel", function (e) {
                handler.call(this, e, BrowserCapability.IsFirefox ? e.detail < 0 : e.wheelDelta > 0);
            }, useCapture);
        },

        /*
        * Method Description:
        * Registers a event handler for text changed event of the specified text element.
        *
        * Parameters:
        * element: a HTML TEXT element(<input type="text" />, <textarea>).
        * handler: The event handler.
        */
        RegisterTextChanged: function (element, handler) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("text element is null or not a object.");
            }
            if (!("value" in element)) {
                throw new Error("element don't has value property.");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("event handler is not a function.");
            }

            var currentValuePropertyName = "Xphter_EventUtility_CurrentValue";
            var textChangedTimerPropertyName = "Xphter_EventUtility_TextChangedTimer";
            var textChangedHandlerPropertyName = "Xphter_EventUtility_TextChangedHandler";

            function InitializeElement(target, handler) {
                target[currentValuePropertyName] = target.value;
                target[textChangedHandlerPropertyName] = handler;

                StartTextChangedTimer(target);
            }

            function StartTextChangedTimer(target) {
                target[currentValuePropertyName] = target.value;

                if (textChangedTimerPropertyName in target) {
                    global.clearTimeout(target[textChangedTimerPropertyName]);
                }

                target[textChangedTimerPropertyName] = global.setTimeout(function () {
                    TextChangedTimerCallback.call(target);
                }, 500);
            }

            function TextChangedTimerCallback() {
                if (this[currentValuePropertyName] != this.value) {
                    this[textChangedHandlerPropertyName].call(this, null, true);
                }

                StartTextChangedTimer(this);
            }

            InitializeElement(element, handler);

            if (BrowserCapability.IsIE) {
                IEEventLayer.GetInstance().Register(element, "propertychange", function () {
                    var e = DocumentUtility.GetWindowObject(this).event;
                    if (e.propertyName == "value") {
                        StartTextChangedTimer(this);

                        handler.apply(this, arguments);
                    }
                }, false);
            } else {
                element.addEventListener("input", function () {
                    StartTextChangedTimer(this);

                    handler.apply(this, arguments);
                }, false);
            }
        },

        /*
        * Method Description:
        * Registers a event handler for selected changed event of the specified dropdown list.
        *
        * Parameters:
        * element: a HTML DROPDOWN element(<select>).
        * handler: The event handler.
        */
        RegisterSelectedChanged: function (element, handler) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("text element is null or not a object.");
            }
            if (!("selectedIndex" in element)) {
                throw new Error("element don't has selectedIndex property.");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("event handler is not a function.");
            }

            var currentSelectedPropertyName = "Xphter_EventUtility_CurrentSelectedIndex";
            var selectedChangedTimerPropertyName = "Xphter_EventUtility_SelectedChangedTimer";
            var selectedChangedHandlerPropertyName = "Xphter_EventUtility_SelectedChangedHandler";

            function InitializeElement(target, handler) {
                target[currentSelectedPropertyName] = target.selectedIndex;
                target[selectedChangedHandlerPropertyName] = handler;

                StartSelectedChangedTimer(target);
            }

            function StartSelectedChangedTimer(target) {
                target[currentSelectedPropertyName] = target.selectedIndex;

                if (selectedChangedTimerPropertyName in target) {
                    global.clearTimeout(target[selectedChangedTimerPropertyName]);
                }

                target[selectedChangedTimerPropertyName] = global.setTimeout(function () {
                    SelectedChangedTimerCallback.call(target);
                }, 500);
            }

            function SelectedChangedTimerCallback() {
                if (this[currentSelectedPropertyName] != this.selectedIndex) {
                    this[selectedChangedHandlerPropertyName].call(this, null, true);
                }

                StartSelectedChangedTimer(this);
            }

            InitializeElement(element, handler);

            if (BrowserCapability.IsIE) {
                IEEventLayer.GetInstance().Register(element, "propertychange", function () {
                    var e = DocumentUtility.GetWindowObject(this).event;
                    if (e.propertyName == "selectedIndex") {
                        StartSelectedChangedTimer(this);

                        handler.apply(this, arguments);
                    }
                }, false);
            } else {
                element.addEventListener("change", function () {
                    StartSelectedChangedTimer(this);

                    handler.apply(this, arguments);
                }, false);
            }
        },

        /*
        * Method Description:
        * Registers a event handler for load event of the specified script element.
        *
        * Parameters:
        * element: a HTML SCRIPT element.
        * handler: The event handler.
        */
        RegisterScriptLoad: function (element, handler) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("script element is null or not a object.");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("event handler is not a function.");
            }

            function OnLoad() {
                if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9 && element.readyState != "loaded" && element.readyState != "complete") {
                    return;
                }

                handler.call(element);
            }

            if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9) {
                IEEventLayer.GetInstance().Register(element, "readystatechange", OnLoad, false);
            } else {
                EventUtility.Register(element, "load", OnLoad, false);
                !BrowserCapability.IsOpera && EventUtility.Register(element, "error", OnLoad, false);
            }
        },

        /*
        * Static Method Description:
        * Unregister a event handler from the specified element.
        * 
        * Parameters:
        * element: The element will be unregistered a event handler.
        * event: The event type without "on" prefix.
        * handler: The event handler.
        * useCapture: Whether unregister a capture handler.
        */
        Unregister: function (element, event, handler, useCapture) {
            if (!DataTypeIdentity.IsNotNullObject(element)) {
                throw new Error("The element which will unregister event handler is undefined");
            }
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The event handler is undefiend");
            }

            if (element.removeEventListener) {
                element.removeEventListener(event, handler, useCapture);
            } else {
                IEEventLayer.GetInstance().Unregister(element, event, handler, useCapture);
            }
        },

        /*
        * Static Method Description:
        * Cancel bubble of the specified event object.
        * 
        * Parameters:
        * e: A event object.
        */
        StopPropagation: function (e) {
            if (e.stopPropagation) {
                e.stopPropagation();
            } else {
                e.cancelBubble = true;
            }
        },

        /*
        * Static Method Description:
        * Prevent the default behavior of the specified event object.
        * 
        * Parameters:
        * e: A event object.
        */
        PreventDefault: function (e) {
            if (e.preventDefault) {
                e.preventDefault();
            } else {
                e.returnValue = false;
            }
        }
    };

    /***************** Event Class Definition *****************/

    /*
    * Class Description:
    * Represents a event object.
    *
    * Constructor Parameters:
    * target: The event source;
    * name : The event name;
    */
    var Event = Xphter.Event = function (target, name) {
        if (!DataTypeIdentity.IsNotNullObject(target)) {
            throw new Error("The event source is undefined.");
        }

        var m_target = target;
        var m_name = name + String.Empty;
        var m_handlers = [];

        /*
        * Method Description:
        * Gets the event target.
        */
        this.GetTarget = function () {
            return m_target;
        };

        /*
        * Method Description:
        * Gets the event name.
        */
        this.GetName = function () {
            return m_name;
        };

        /*
        * Method Description:
        * Registers a handler of this event.
        *
        * Parameters:
        * handler: A callback function which will be call by event target when this event occurred.
        */
        this.AddListener = function (handler) {
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The callback function that will be called by the event target is undefined.");
            }

            m_handlers.push(handler);
        };

        /*
        * Method Description:
        * Unregisters a handler of this event.
        *
        * Parameters:
        * handler: The callback function which has been registered for this event.
        */
        this.RemoveListener = function (handler) {
            if (!DataTypeIdentity.IsFunction(handler)) {
                throw new Error("The callback function that will be called by the event target is undefined.");
            }

            for (var i = 0; i < m_handlers.length; i++) {
                if (m_handlers[i] == handler) {
                    m_handlers.splice(i, 1);
                    break;
                }
            }
        };

        /*
        * Method Description:
        * Unregisters all handlers of this event.
        */
        this.RemoveListeners = function () {
            m_handlers.length = 0;
        };

        /*
        * Method Description:
        * Raises this event, call all event handlers used event target.
        * All input parameters will pass to event handlers.
        */
        this.Raise = function () {
            if (!DataTypeIdentity.IsNotNullObject(m_target)) {
                return;
            }

            //collect event arguments
            var paramters = [];
            for (var i = 0; i < arguments.length; i++) {
                paramters.push(arguments[i]);
            }

            //call event handlers
            for (var i = 0; i < m_handlers.length; i++) {
                m_handlers[i].apply(m_target, paramters);
            }
        }
    };

    /***************** KeyCodes Class Definition *****************/

    /*
    * Static Class Description:
    * Provides the standard key codes.
    */
    var KeyCodes = Xphter.KeyCodes = {
        Return: 0x0D,

        NewLine: 0x0A,

        Left: 0x25,

        Up: 0x26,

        Right: 0x27,

        Down: 0x28
    };

    /***************** HttpMethods Class Definition *****************/

    /*
    * Static Class Description:
    * Provides the standard HTTP methods.
    */
    var HttpMethods = Xphter.HttpMethods = {
        /*
        * Represnets an HTTP OPTIONS protocol method.
        */
        Options: "OPTIONS",

        /*
        * Represnets an HTTP TRACE protocol method.
        */
        Trace: "TRACE",

        /*
        * Represnets an HTTP HEAD protocol method.
        */
        Head: "HEAD",

        /*
        * Represnets an HTTP GET protocol method.
        */
        Get: "GET",

        /*
        * Represnets an HTTP POST protocol method.
        */
        Post: "POST",

        /*
        * Represnets an HTTP PUT protocol method.
        */
        Put: "PUT",

        /*
        * Represnets an HTTP DELETE protocol method.
        */
        Delete: "DELETE"
    };

    /***************** AjaxUtility Class Definition *****************/

    /*
    * Static Class Description:
    * Provides helper method for ajax.
    */
    var AjaxUtility = Xphter.AjaxUtility = {
        /*
        * Method Description:
        * Create a XMLHttpRequest object.
        *
        * Return:
        * Returns a XMLHttpRequest object or null if browser is not support ajax.
        */
        CreateRequest: function () {
            var request = null;

            if (global.XMLHttpRequest) {
                request = new global.XMLHttpRequest();
            } else if (global.ActiveXObject) {
                try {
                    request = new global.ActiveXObject("Microsoft.XMLHTTP");
                } catch (ex) {
                }
            }

            return request;
        },

        /*
        * Method Description:
        * Gets the HTTP body by the specified data which will send to a HTTP server.
        *
        * Return:
        * Returns the HTTP string body.
        */
        GetHttpBody: function (data) {
            var body = undefined;

            if (DataTypeIdentity.IsUndefinedOrNull(data)) {
                // keep undefined default
            } else if (DataTypeIdentity.IsObject(data)) {
                body = Uri.EncodeObjectToHttpBody(data);
            } else if (DataTypeIdentity.IsString(data)) {
                body = data;
            } else {
                body = data.toString();
            }

            return body;
        },

        /*
        * Method Description:
        * Gets response from a XMLHttpRequest object.
        * 1. The responseXML property is available:
        * Return the responseXML property.
        *
        * 2. The responseText property is available:
        * If the Content-Type header is text/json or text/x-json or application/json
        * or application/x-json, then try to eval the responseText property to a object
        * and return it, else return the responseText property.
        *
        * 3. Other cases:
        * Return this XMLHttpRequest object.
        * 
        *
        * Return:
        * Returns a object from this XMLHttpRequest object.
        */
        GetResponseFromRequest: function (request) {
            if (!DataTypeIdentity.IsNotNullObject(request)) {
                throw new Error("XMLHttpRequest object is undefined.");
            }

            if (request.responseXML && request.responseXML.documentElement) {
                return request.responseXML;
            } else if (request.responseText) {
                var response = request.responseText;
                if ((/text\/json|text\/x\-json|application\/json|application\/x\-json/i).test(request.getResponseHeader("Content-Type"))) {
                    try {
                        response = global.eval("(" + response + ")");
                    } catch (ex) {
                    }
                }

                return response;
            } else {
                return request;
            }
        },

        /*
        * Method Description:
        * Synchronous invoke a resource, a error will throw if this is a cross-domain invoke.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   Uri: The resource URI.
        *   Method: The HTTP method.
        *   Data: The post data.
        *   Header: A object which properties are the HTTP headers will be sent.
        *   OnLoad: The callback function when load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnSuccess: The callback function when successfully load resource. The response object will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnComplete: The callback function when complete load resource.
        * }
        *
        */
        Sync: function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }

            //check whether the resource URI is legal
            var uri = new Uri((parameters.Uri || String.Empty) + String.Empty);
            if (!uri.GetIsWellFormat()) {
                throw new Error("Invalid URI string.");
            }

            //check whether this is a cross-domain invoke
            if (!uri.GetIsRelative() && (!uri.GetHost().EqualsIgnoreCase(global.location.host))) {
                throw new Error("Cross-domain invoke is not supported, please call CrossDomain method instead.");
            }

            var request = AjaxUtility.CreateRequest();
            if (request) {
                //build request
                request.open(((parameters.Method || HttpMethods.Get) + String.Empty).toUpperCase(), uri.GetOriginalString(), false);
                request.setRequestHeader("Cache-Control", "no-cache");
                request.setRequestHeader("If-Modified-Since", "0");
                if (parameters.Header) {
                    for (var name in parameters.Header) {
                        name && request.setRequestHeader(name, parameters.Header[name] + String.Empty);
                    }
                }

                //send request
                request.send(AjaxUtility.GetHttpBody(parameters.Data));

                //process response
                var response = AjaxUtility.GetResponseFromRequest(request);
                parameters.OnLoad && parameters.OnLoad(request);
                if (request.status >= 200 && request.status < 300) {
                    parameters.OnSuccess && parameters.OnSuccess(response);
                } else {
                    parameters.OnError && parameters.OnError(request);
                }
                parameters.OnComplete && parameters.OnComplete();
            }
        },

        /*
        * Method Description:
        * Asynchronous invoke a resource, a error will throw if this is a cross-domain invoke.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   Uri: The resource URI.
        *   Method: The HTTP method.
        *   Data: The post data.
        *   Header: A object which properties are the HTTP headers will be sent.
        *   OnLoad: The callback function when load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnSuccess: The callback function when successfully load resource. The response object will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnTimeout: The callback function when load resource timeout.
        *   OnComplete: The callback function when complete load resource.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        Async: function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }

            //check whether the resource URI is legal
            var uri = new Uri((parameters.Uri || String.Empty) + String.Empty);
            if (!uri.GetIsWellFormat()) {
                throw new Error("Invalid URI string.");
            }

            //check whether this is a cross-domain invoke
            if (!uri.GetIsRelative() && (!uri.GetHost().EqualsIgnoreCase(global.location.host))) {
                throw new Error("Cross-domain invoke is not supported, please call CrossDomain method instead.");
            }

            var request = AjaxUtility.CreateRequest();
            if (request) {
                var isTimeout = false;
                var isRecevied = false;
                var timeoutTimer = null;
                var timeout = !global.isNaN(parameters.Timeout) ? parameters.Timeout - 0 : 0;

                //build request
                request.open(((parameters.Method || HttpMethods.Get) + String.Empty).toUpperCase(), uri.GetOriginalString(), true);
                request.setRequestHeader("Cache-Control", "no-cache");
                request.setRequestHeader("If-Modified-Since", "0");
                if (parameters.Header) {
                    for (var name in parameters.Header) {
                        name && request.setRequestHeader(name, parameters.Header[name] + String.Empty);
                    }
                }
                request.onreadystatechange = function () {
                    if (request.readyState != 4 || isTimeout) {
                        return;
                    }

                    isRecevied = true;
                    //clear timeout timer
                    if (timeoutTimer) {
                        global.clearTimeout(timeoutTimer);
                        timeoutTimer = null;
                    }

                    //process response
                    var response = AjaxUtility.GetResponseFromRequest(request);
                    parameters.OnLoad && parameters.OnLoad(request);
                    if (request.status >= 200 && request.status < 300) {
                        parameters.OnSuccess && parameters.OnSuccess(response);
                    } else {
                        parameters.OnError && parameters.OnError(request);
                    }
                    parameters.OnComplete && parameters.OnComplete();
                };

                //set timeout
                if (timeout > 0) {
                    timeoutTimer = global.setTimeout(function () {
                        if (isRecevied) {
                            return;
                        }

                        isTimeout = true;

                        //abort request            
                        request.abort();

                        //clear timeout timer
                        global.clearTimeout(timeoutTimer);
                        timeoutTimer = null;

                        //process error
                        parameters.OnLoad && parameters.OnLoad(request);
                        parameters.OnTimeout && parameters.OnTimeout();
                        parameters.OnComplete && parameters.OnComplete();
                    }, timeout);
                }

                //send request
                request.send(AjaxUtility.GetHttpBody(parameters.Data));
            }
        },

        /*
        * Method Description:
        * Asynchronous postback a form which contains file-upload elements.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   Form: The form to postback.
        *   OnLoad: The callback function when load resource.
        *   OnSuccess: The callback function when successfully load resource. The response object or the returned HTML text will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A Error object argument will pass to this method.
        *   OnTimeout: The callback function when load resource timeout.
        *   OnComplete: The callback function when complete load resource.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        Upload: function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }

            var isTimeout = false;
            var isRecevied = false;
            var timeoutTimer = null;
            var timeout = !global.isNaN(parameters.Timeout) ? parameters.Timeout - 0 : 0;

            //create the target iframe
            var frame = null;
            var frameName = new Date().getTime().toString();
            if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9) {
                /*
                * IE6/7/8 will ignore all setter of properties of iframe object.
                */
                frame = DocumentUtility.CreateElement(String.Format('<iframe name="{0}" src="about:blank" style="width: 0px; height: 0px; border-style: none; visibility: hidden;">', frameName));
            } else {
                frame = DocumentUtility.CreateElement("iframe", {
                    name: frameName,
                    src: "about:blank"
                }, {
                    width: "0px",
                    height: "0px",
                    borderStyle: "none",
                    visibility: "hidden"
                });
            }
            global.document.body.appendChild(frame);
            EventUtility.Register(frame, "load", function () {
                if (isTimeout) {
                    return;
                }

                isRecevied = true;
                //clear timeout timer                    
                if (timeoutTimer) {
                    global.clearTimeout(timeoutTimer);
                    timeoutTimer = null;
                }

                var response = frame.contentWindow.document.body.innerHTML;
                try {
                    response = global.eval("(" + response + ")");
                } catch (ex) {
                }

                try {
                    parameters.OnLoad && parameters.OnLoad();
                    parameters.OnSuccess && parameters.OnSuccess(response);
                    parameters.OnComplete && parameters.OnComplete();
                } finally {
                    global.setTimeout(function () {
                        frame.parentNode.removeChild(frame);
                    }, 0);
                }
            }, false);

            //submit form to this iframe
            try {
                parameters.Form.target = frame.name;
                parameters.Form.submit();
            } catch (ex) {
                frame.parentNode.removeChild(frame);
                parameters.OnLoad && parameters.OnLoad();
                parameters.OnError && parameters.OnError(ex);
                parameters.OnComplete && parameters.OnComplete();
                return;
            }

            //start a timerout timer
            if (timeout > 0) {
                timeoutTimer = global.setTimeout(function () {
                    if (isRecevied) {
                        return;
                    }

                    isTimeout = true;
                    //clear timeout timer
                    global.clearTimeout(timeoutTimer);
                    timeoutTimer = null;

                    //process error
                    frame.parentNode.removeChild(frame);
                    parameters.OnLoad && parameters.OnLoad();
                    parameters.OnTimeout && parameters.OnTimeout();
                    parameters.OnComplete && parameters.OnComplete();
                }, timeout);
            }
        },

        /*
        * Method Description:
        * Asynchronous postback a form, a error will throw if this is a cross-domain postback.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   Form: The target form.
        *   OnLoad: The callback function when load resource.
        *   OnSuccess: The callback function when successfully load resource. The response object or response HTML text will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A error message argument will pass to this method.
        *   OnTimeout: The callback function when load resource timeout.
        *   OnComplete: The callback function when complete load resource.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        Postback: function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }

            var form = parameters.Form;
            if ("multipart/form-data".EqualsIgnoreCase(form.enctype)) {
                AjaxUtility.Upload({
                    Form: form,
                    OnLoad: parameters.OnLoad,
                    OnSuccess: parameters.OnSuccess,
                    OnError: parameters.OnError ? function (error) {
                        parameters.OnError(error.message);
                    } : null,
                    OnTimeout: parameters.OnTimeout,
                    OnComplete: parameters.OnComplete,
                    Timeout: parameters.Timeout
                });
            } else {
                AjaxUtility.Async({
                    Uri: form.action,
                    Method: form.method,
                    Data: DocumentUtility.EncodeFormToHttpBody(form),
                    Header: {
                        "Content-Type": form.enctype
                    },
                    OnLoad: parameters.OnLoad,
                    OnSuccess: parameters.OnSuccess,
                    OnError: parameters.OnError ? function (request) {
                        parameters.OnError(request.responseText);
                    } : null,
                    OnTimeout: parameters.OnTimeout,
                    OnComplete: parameters.OnComplete,
                    Timeout: parameters.Timeout
                });
            }
        },

        /*
        * Method Description:
        * Asynchronous invoke a resource within a different domain.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   Uri: The resource URI.
        *   Charset: The encoding of response data.
        *   OnComplete: The callback function when completely load resource.
        *   OnTimeout: The callback function when load resource timeout.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        * Mask:
        * There is not a OnError function in parameters object, so caller should process error in OnComplete or OnTimeout function.
        *
        */
        CrossDomain: function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }

            //check whether the resource URI is legal
            var uri = new Uri((parameters.Uri || String.Empty) + String.Empty);
            if (!uri.GetIsWellFormat()) {
                return;
            }

            //function used to remove script element.
            function RemoveScript() {
                if (!script) {
                    return;
                }

                if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9) {
                    EventUtility.Unregister(script, "readystatechange", ProcessResponse, false);
                } else {
                    EventUtility.Unregister(script, "load", ProcessResponse, false);
                    !BrowserCapability.IsOpera && EventUtility.Unregister(script, "error", ProcessResponse, false);
                }
                script.src = null;
                script.parentNode && script.parentNode.removeChild(script);
                script = null;
            }

            //function used to process response.
            function ProcessResponse() {
                if (isTimeout || isProcessed) {
                    return;
                }

                isProcessed = true;

                //clear timeout timer
                if (timeoutTimer) {
                    global.clearTimeout(timeoutTimer);
                    timeoutTimer = null;
                }

                //remove script
                RemoveScript();

                //invoke callback
                parameters.OnComplete && parameters.OnComplete();
            }

            var isTimeout = false;
            var isProcessed = false;
            var timeoutTimer = null;
            var timeout = !global.isNaN(parameters.Timeout) ? parameters.Timeout - 0 : 0;

            //create script element
            var script = DocumentUtility.CreateElement("script", {
                type: "text/javascript",
                charset: (parameters.Charset || "UTF-8") + String.Empty,
                src: uri.GetOriginalString()
            });
            EventUtility.RegisterScriptLoad(script, ProcessResponse);

            //set timeout
            if (timeout > 0) {
                timeoutTimer = global.setTimeout(function () {
                    if (isProcessed) {
                        return;
                    }

                    isTimeout = true;

                    //clear timeout timer
                    global.clearTimeout(timeoutTimer);
                    timeoutTimer = null;

                    //remove script
                    RemoveScript();

                    //invoke callback
                    parameters.OnTimeout && parameters.OnTimeout();
                }, timeout);
            }

            //send request
            global.document.getElementsByTagName("head")[0].appendChild(script);
        }
    };

    /***************** ValidatedForm Class Definition *****************/

    /*
    * Class Description:
    * Provides the validation functions to a HTML form.
    *
    * Constructor Parameters:
    * form: The target form.
    * 
    * Remark:
    * The validation options format: group:'groupName',nullable:true|false,rule:'ruleName',checkNull:true|false,currentID:'a element ID, null use the target form element',warningClass:'a css class',warningID:'a element ID';
    * The remote validation rule should be same in one group to avoid form status confusion.
    */
    var ValidatedForm = Xphter.ValidatedForm = function (form) {
        var m_form;
        var m_textChangedTimeout = 500;

        // all validating elements: Target, Validator, CurrentID, WarningClass, WarningID
        var m_elements = [];

        var m_optionsAttributeName = "validate-options";
        var m_groupPropertyName = "group";
        var m_nullPropertyName = "nullable";
        var m_rulePropertyName = "rule";
        var m_checkNullPropertyName = "checkNull";
        var m_currentPropertyName = "currentID";
        var m_warningClassPropertyName = "warningClass";
        var m_warningIDPropertyName = "warningID";

        var m_elementIndexAttributeName = "Xphter_ValidatedForm_ElementIndex";
        var m_textChangedTimerPropertyName = "Xphter_ValidatedForm_TextChangedTimer";

        var m_isSubmiting = false;
        var m_validateCallbak = null;

        /*
        * provides a validator implementation for a common function.
        */
        var FunctionValidator = function (func) {
            var m_func = func;

            this.Validate = function (target, values, context) {
                m_func(target, values, context);
            };
        };

        /*
        * provides a validator implementation for validate whether allow empty value.
        */
        var EmptyValidator = function (required, checkNull, validator) {
            var m_required = required;
            var m_checkNull = checkNull;
            var m_validator = validator;

            this.Validate = function (target, values, context) {
                var isEmpty = !values.length || values.All(function (item) {
                    return item.IsEmptyOrWhiteSpace();
                });

                if (isEmpty) {
                    if (m_checkNull && m_validator) {
                        m_validator.Validate(target, values, context);
                    } else {
                        context.SetStatus(!m_required);
                    }
                } else {
                    if (m_validator) {
                        m_validator.Validate(target, values, context);
                    } else {
                        context.SetStatus(true);
                    }
                }
            };
        };

        /*
        * provides the default implementation of validator context object.
        */
        var ValidatorContext = function (element) {
            var m_element = element;
            var m_nullable = element.Nullable;
            var m_warningClass = element.WarningClass;
            var m_currentElement = (element.CurrentID ? global.document.getElementById(element.CurrentID) : null) || element.Target;
            var m_warningElement = element.WarningID ? global.document.getElementById(element.WarningID) : null;

            this.Nullable = m_nullable;
            this.WarningClass = m_warningClass;
            this.CurrentElement = m_currentElement;
            this.WarningElement = m_warningElement;

            this.SetStatus = function (status) {
                m_element.Status = !!status;

                if (m_element.Status) {
                    m_warningClass && DocumentUtility.RemoveCssClass(m_currentElement, m_warningClass);
                    m_warningElement && DocumentUtility.HideElement(m_warningElement);
                } else {
                    m_warningClass && DocumentUtility.AddCssClass(m_currentElement, m_warningClass);
                    m_warningElement && DocumentUtility.ShowElement(m_warningElement);

                    m_element.Target.focus();
                }

                PerformCallback();
            };
        };

        var AggregateValidatorContext = function (element, elements) {
            var m_element = element;
            var m_nullable = element.Nullable;
            var m_warningClass = element.WarningClass;
            var m_currentElement = (element.CurrentID ? global.document.getElementById(element.CurrentID) : null) || element.Target;
            var m_warningElement = element.WarningID ? global.document.getElementById(element.WarningID) : null;

            var m_contexts = elements.Select(function (item) {
                return new ValidatorContext(item);
            });

            this.Nullable = m_nullable;
            this.WarningClass = m_warningClass;
            this.CurrentElement = m_currentElement;
            this.WarningElement = m_warningElement;

            this.SetStatus = function (status) {
                for (var i = 0; i < m_contexts.length; i++) {
                    m_contexts[i].SetStatus(status);
                }
            };
        };

        Constructor();

        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(form)) {
                throw new Error("form is undefined.");
            }
            if (!("elements" in form)) {
                throw new Error("form is not a HTML Form element");
            }

            m_form = form;

            Initialize(m_form);
        }

        function GetTargetValues(target) {
            var values = [];
            var option = null;

            switch (target.type.toLowerCase()) {
                case "hidden":
                case "text":
                case "textarea":
                case "password":
                case "email":
                case "url":
                case "number":
                case "search":
                case "color":
                case "range":
                case "date":
                case "month":
                case "week":
                case "time":
                case "datetime":
                case "datetime-local":
                    values.push(target.value);
                    break;
                case "checkbox":
                case "radio":
                    if (target.checked) {
                        values.push(target.value);
                    }
                    break;
                case "select-one":
                case "select-multiple":
                    for (var j = 0; j < target.options.length; j++) {
                        if (!(option = target.options[j]).selected) {
                            continue;
                        }

                        values.push(option.value);
                    }
                    break;
            }

            return values;
        }

        function GetTargetIndex(target) {
            var value = target.getAttribute(m_elementIndexAttributeName);
            var index = global.parseInt(value);

            if (global.isNaN(index)) {
                throw new Error(String.Format("Invalid element index attribute value: {0}", value));
            }
            if (index < 0 || index >= m_elements.length) {
                throw new Error("Element index is out of range.");
            }

            return index;
        }

        function ClearStatus(element) {
            var elements = element.Group ? m_elements.Where(function (item) {
                return item.Group == element.Group;
            }) : [element];

            for (var i = 0; i < elements.length; i++) {
                elements[i].Status = null;
            }

            return elements;
        }

        function PerformValidate(element) {
            // clear status before validating
            var elements = ClearStatus(element);

            // execute validator
            element.Validator.Validate(element.Target, GetTargetValues(element.Target), new AggregateValidatorContext(element, elements));
        }

        function FireValidate() {
            var index = GetTargetIndex(this);

            PerformValidate(m_elements[index]);
        }

        function DelayFireValidate(target) {
            // clear status immediately
            var index = GetTargetIndex(target);
            ClearStatus(m_elements[index]);

            if (target[m_textChangedTimerPropertyName]) {
                window.clearTimeout(target[m_textChangedTimerPropertyName]);
            }

            target[m_textChangedTimerPropertyName] = window.setTimeout(function () {
                delete target[m_textChangedTimerPropertyName];

                FireValidate.call(target);
            }, m_textChangedTimeout);
        }

        function ResetValidateStatus(element) {
            element.Status = null;

            var currentElement = (element.CurrentID ? global.document.getElementById(element.CurrentID) : null) || element.Target;
            var warningElement = element.WarningID ? global.document.getElementById(element.WarningID) : null;

            element.WarningClass && DocumentUtility.RemoveCssClass(currentElement, element.WarningClass);
            warningElement && DocumentUtility.HideElement(warningElement);

            return element;
        }

        function Initialize(form) {
            var data = [];
            var currentElement = null;
            var existingElement = null;
            var validateOptionsValue = null, validateOptions = null;
            var groupName = null, ruleName = null, ruleObj = null, required = false, checkNull = false, validator = null;

            for (var i = 0; i < form.elements.length; i++) {
                currentElement = form.elements[i];
                if (!("type" in currentElement) || !currentElement.name) {
                    continue;
                }
                if (!(validateOptionsValue = currentElement.getAttribute(m_optionsAttributeName))) {
                    continue;
                }

                try {
                    validateOptions = window.eval(String.Format("({{ {0} }})", validateOptionsValue));
                } catch (ex) {
                    throw new Error(String.Format("{0} not represents a valid validation options.", validateOptionsValue));
                }
                if (!(m_groupPropertyName in validateOptions)) {
                    validateOptions[m_groupPropertyName] = null;
                }
                if (!(m_nullPropertyName in validateOptions)) {
                    // default value of nullable property is true.
                    validateOptions[m_nullPropertyName] = true;
                }

                validator = null;
                groupName = validateOptions[m_groupPropertyName];
                ruleName = validateOptions[m_rulePropertyName];
                required = !validateOptions[m_nullPropertyName];
                checkNull = !!validateOptions[m_checkNullPropertyName];
                if (!ruleName && !required) {
                    continue;
                }

                existingElement = m_elements.Where(function (item) {
                    return item.Target == currentElement;
                }).FirstOrDefault();

                if (!existingElement) {
                    switch (currentElement.type.toLowerCase()) {
                        case "hidden":
                        case "text":
                        case "textarea":
                        case "password":
                        case "email":
                        case "url":
                        case "search":
                            EventUtility.RegisterTextChanged(currentElement, function (event, isProgrammatic) {
                                if (isProgrammatic) {
                                    FireValidate.call(this);
                                } else {
                                    DelayFireValidate(this);
                                }
                            });
                            break;
                        case "number":
                        case "color":
                        case "range":
                        case "date":
                        case "month":
                        case "week":
                        case "time":
                        case "datetime":
                        case "datetime-local":
                            EventUtility.Register(currentElement, "change", function () {
                                FireValidate.call(this);
                            }, false);
                            break;
                        case "checkbox":
                        case "radio":
                            EventUtility.Register(currentElement, BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 9 ? "click" : "change", function () {
                                FireValidate.call(this);
                            }, false);
                            break;
                            break;
                        case "select-one":
                        case "select-multiple":
                            EventUtility.RegisterSelectedChanged(currentElement, function () {
                                FireValidate.call(this);
                            }, false);
                            break;
                    }
                }

                if (ruleName) {
                    ruleObj = ValidatedForm.g_rules.Where(function (item) {
                        return item.IsMatch(ruleName);
                    }).FirstOrDefault();
                    if (ruleObj != null) {
                        validator = ruleObj.GetValidator(ruleName);
                    } else {
                        validator = global[ruleName];
                        if (!DataTypeIdentity.IsFunction(validator)) {
                            throw new Error(String.Format("{0} is not a function.", ruleName));
                        }

                        validator = new FunctionValidator(validator);
                    }

                    if (!validator) {
                        throw new Error(String.Format("{0} is not a valid rule name.", ruleName));
                    }
                    if (!IFormValidator.IsImplementBy(validator)) {
                        throw new Error(String.Format("The validator of {0} is not a IFormValidator object.", ruleName));
                    }
                }

                currentElement.setAttribute(m_elementIndexAttributeName, data.length.toString());
                data.push(ResetValidateStatus({
                    Target: currentElement,
                    Validator: new EmptyValidator(required, checkNull, validator),
                    Group: groupName,
                    Nullable: !required,
                    CurrentID: validateOptions[m_currentPropertyName],
                    WarningClass: validateOptions[m_warningClassPropertyName],
                    WarningID: validateOptions[m_warningIDPropertyName]
                }));
            }

            m_elements = data;
        }

        function PerformCallback() {
            if (m_validateCallbak && m_elements.All(function (item) {
                return item.Status != null;
            })) {
                try {
                    m_validateCallbak(m_elements.Where(function (item) {
                        return !item.Status;
                    }).Select(function (item) {
                        return item.Target;
                    }));
                } finally {
                    m_validateCallbak = null;
                }
            }
        }

        function StartValidation() {
            var element = null;

            for (var i = 0; i < m_elements.length; i++) {
                element = m_elements[i];

                if (element.Status == null) {
                    PerformValidate(element);
                } else if (!element.Status) {
                    element.Target.focus();
                }
            }

            PerformCallback();
        }

        function SubmitForm(parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("parameters is undefined.");
            }

            if (m_isSubmiting) {
                return;
            }

            m_isSubmiting = true;
            m_validateCallbak = function (invalidElements) {
                if (!invalidElements.length) {
                    AjaxUtility.Postback({
                        Form: m_form,
                        OnLoad: parameters.OnLoad,
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnTimeout: parameters.OnTimeout,
                        OnComplete: function () {
                            try {
                                parameters.OnComplete && parameters.OnComplete();
                            } finally {
                                m_isSubmiting = false;
                            }
                        },
                        Timeout: parameters.Timeout
                    });
                } else {
                    m_isSubmiting = false;
                }
            };

            StartValidation();
        }

        function ValidateForm(parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("parameters is undefined.");
            }

            m_validateCallbak = function (invalidElements) {
                if (!invalidElements.length) {
                    parameters.OnSuccess && parameters.OnSuccess();
                } else {
                    parameters.OnFail && parameters.OnFail(invalidElements);
                }
            };

            StartValidation();
        }

        /*
        * Method Description:
        * Reload status of current form.
        */
        this.Refresh = function () {
            Initialize(m_form);
        };

        /*
        * Method Description:
        * Asynchronous postback a form, a error will throw if this is a cross-domain postback.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   OnLoad: The callback function before to submit.
        *   OnSuccess: The callback function when success to submit. The response object or response HTML text will pass to this method.
        *   OnError: The callback function when fail to submit. A error message argument will pass to this method.
        *   OnTimeout: The callback function when submit timeout.
        *   OnComplete: The callback function when complete to submit.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        this.Submit = SubmitForm;

        /*
        * Method Description:
        * Validates this form.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   OnSuccess: The callback function invoked when all form elements are valid.
        *   OnFail: The callback function invoked when any form element is invalid. The invalid elements will pass to this function.
        * }
        *
        */
        this.Validate = ValidateForm;
    };

    /*
    * Interface Description:
    * Represents a validation rule.
    * }
    */
    var IFormValidationRule = Xphter.IFormValidationRule = {
        IsImplementBy: IInterface.IsImplementBy,

        /*
        * Method Description:
        * Gets a value to indicate whether the specified rule name is matched this rule.
        */
        IsMatch: function (name) {
            throw new Error("Not Implementation.");
        },

        /*
        * Method Description:
        * Gets a IFormValidator from the specified rule name.
        */
        GetValidator: function (name) {
            throw new Error("Not Implementation.");
        }
    };

    /*
    * Interface Description:
    * Represents a validator.
    * }
    */
    var IFormValidator = Xphter.IFormValidator = {
        IsImplementBy: IInterface.IsImplementBy,

        /*
        * Method Description:
        * Validates current values of the specified target element.
        */
        Validate: function (target, values, context) {
            throw new Error("Not Implementation.");
        }
    };

    // internal validation rules
    ValidatedForm.g_rules = [];

    /*
    * Static method Description:
    * Adds a validation rule.
    */
    ValidatedForm.AddRule = function (rule) {
        if (!DataTypeIdentity.IsNotNullObject(rule)) {
            throw new Error("rule is undefined.");
        }

        if (!IFormValidationRule.IsImplementBy(rule)) {
            throw new Error("rule is not a IFormValidationRule object.");
        }

        ValidatedForm.g_rules.push(rule);
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate length.
    */
    var LengthFormValidationRule = Xphter.LengthFormValidationRule = function () {
        var m_rulePattern = /^length\((\d+)(?:\-(\d+))*\)$/gi;

        var Validator = function (min, max) {
            if (global.isNaN(min)) {
                throw new Error("min is not a number");
            }
            if (global.isNaN(max)) {
                throw new Error("max is not a number");
            }

            var m_minLength = min - 0;
            var m_maxLength = max - 0;

            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                var item = null;
                var result = true;
                for (var i = 0; i < values.length; i++) {
                    item = values[i].toString();

                    if (item.length < m_minLength || item.length > m_maxLength) {
                        result = false;
                        break;
                    }
                }

                context.SetStatus(result);
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            m_rulePattern.lastIndex = 0;
            return m_rulePattern.test(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            m_rulePattern.lastIndex = 0;
            var match = m_rulePattern.exec(name.toString());
            if (match == null) {
                throw new Error("unmatched rule name");
            }

            var min = global.parseInt(match[1]);
            var max = match.length > 1 && match[2] ? global.parseInt(match[2]) : Number.MAX_VALUE;

            return new Validator(min, max);
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a e-mail.
    */
    var EmailFormValidationRule = Xphter.EmailFormValidationRule = function () {
        var m_ruleName = "email";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsEmail());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a integer.
    */
    var IntegerFormValidationRule = Xphter.IntegerFormValidationRule = function () {
        var m_ruleName = "integer";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsInteger());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a number.
    */
    var NumberFormValidationRule = Xphter.NumberFormValidationRule = function () {
        var m_ruleName = "number";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsNumber());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a DateTime.
    */
    var DateTimeFormValidationRule = Xphter.DateTimeFormValidationRule = function () {
        var m_ruleName = "datetime";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsDateTime());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a mobile number.
    */
    var PhoneNumberFormValidationRule = Xphter.PhoneNumberFormValidationRule = function () {
        var m_ruleName = "phone";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsPhoneNumber());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    /*
    * Class Description:
    * An internal IFormValidationRule implementation: validate whether is a IPv4 address .
    */
    var IPv4FormValidationRule = Xphter.IPv4FormValidationRule = function () {
        var m_ruleName = "ipv4";

        var Validator = function () {
            this.Validate = function (target, values, context) {
                if (!values.length) {
                    throw new Error("The length of values is zero or undefined.");
                }

                context.SetStatus(values[0].IsIPv4());
            };
        };

        this.IsMatch = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return m_ruleName.EqualsIgnoreCase(name);
        };

        this.GetValidator = function (name) {
            if (!name) {
                throw new Error("name is undefined.");
            }

            return new Validator();
        };
    };

    // Add internal IFormValidationRule objects
    ValidatedForm.AddRule(new LengthFormValidationRule());
    ValidatedForm.AddRule(new EmailFormValidationRule());
    ValidatedForm.AddRule(new IntegerFormValidationRule());
    ValidatedForm.AddRule(new NumberFormValidationRule());
    ValidatedForm.AddRule(new DateTimeFormValidationRule());
    ValidatedForm.AddRule(new PhoneNumberFormValidationRule());
    ValidatedForm.AddRule(new IPv4FormValidationRule());

    /***************** XmlUtility Class Definition *****************/

    /*
    * Static Class Description:
    * Provides helper method for processing XML document.
    */
    var XmlUtility = Xphter.XmlUtility = {
        /*
        * Method Description:
        * Creates a empty XML document.
        *
        * Return:
        * Returns a Document object.
        */
        CreateDocument: function () {
            var result = null;

            if (global.document.implementation && global.document.implementation.createDocument) {
                result = global.document.implementation.createDocument(null, null, null);
            } else if (global.ActiveXObject) {
                result = new global.ActiveXObject("Microsoft.XMLDOM");
            }

            return result;
        },

        /*
        * Method Description:
        * Create a XML element with the specified namespace.
        * IE6 don't support the standard method: document.createElementNS.
        *
        * Parameters:
        * xmlDocument: A XML document.
        * name: The element name.
        * namespaceURL: The namespace URL.
        *
        * Return:
        * Returns a Document object.
        */
        CreateElementNS: function (xmlDocument, name, namespaceURL) {
            if (!xmlDocument) {
                throw new Error("XML document is undefined.");
            }
            if (!name) {
                throw new Error("Element name is undefined.");
            }

            var node = null;
            var _name = name + String.Empty;

            if (namespaceURL) {
                if (xmlDocument.createElementNS) {
                    node = xmlDocument.createElementNS(namespaceURL + String.Empty, _name);
                } else {
                    node = xmlDocument.createElement(_name);
                    var match = (/^([^\:]+)\:/i).exec(_name);
                    if (match) {
                        node.setAttribute("xmlns:" + match[1], namespaceURL + String.Empty);
                    } else {
                        node.setAttribute("xmlns", namespaceURL + String.Empty);
                    }
                }
            } else {
                node = xmlDocument.createElement(_name);
            }

            return node;
        },

        /*
        * Method Description:
        * Load a XML document from the specified string.
        *
        * Parameters:
        * xml: The XML string.
        *
        * Return:
        * Returns a Document object.
        */
        LoadString: function (xml) {
            if (!xml) {
                throw new Error("XML string is undefined.");
            }

            var result = null;
            var _xml = xml + String.Empty;

            if (global.DOMParser) {
                result = new global.DOMParser().parseFromString(_xml, "text/xml");
            } else if (global.ActiveXObject) {
                var result = new global.ActiveXObject("Microsoft.XMLDOM");
                result.async = false;
                result.loadXML(_xml);
            }

            return result;
        },

        /*
        * Method Description:
        * Load a XML document from the specified file in local domain.
        *
        * Parameters:
        * file: The file path.
        *
        * Return:
        * Returns a Document object.
        */
        LoadFile: function (file) {
            if (!file) {
                throw new Error("File URL is undefined.");
            }
            var url = new Uri(file);
            if (!url.GetIsWellFormat()) {
                throw new Error("Invalid URI string.");
            }
            if (!url.GetIsRelative() && (!url.GetHost().EqualsIgnoreCase(global.location.host))) {
                throw new Error("Cross domian is not supported.");
            }

            var result = null;

            AjaxUtility.Sync({
                Uri: url.GetOriginalString(),
                Method: "GET",
                OnSuccess: function (xml) {
                    result = xml;
                }
            });

            return result;
        },

        /*
        * Method Description:
        * Gets XML string from the specified node.
        *
        * Parameters:
        * xml: A XML node.
        *
        * Return:
        * Returns XML string.
        */
        GetString: function (node) {
            if (!DataTypeIdentity.IsNotNullObject(node)) {
                throw new Error("Node is undefined.");
            }

            var xml = null;

            if (node.xml) {
                xml = node.xml;
            } else if (global.XMLSerializer) {
                xml = new global.XMLSerializer().serializeToString(node);
            }

            return xml;
        },

        /*
        * Method Description:
        * Executes a XPath query and returns the first match node.
        * 
        * Parameters:
        * node: The executing context of XPath query.
        * queryString : A XPath query string;
        * namespaces: A object. The name of a property in this object is a XML namespace prefix and it's value is a XML namespace URL.
        *
        * Returns:
        * Return the first match node.
        */
        SelectSingleNode: function (node, queryString, namespaces) {
            if (!node) {
                throw new Error("Node is undefined.");
            }
            if (!queryString) {
                throw new Error("XPath query string is undefined.");
            }

            var result = null;
            var doc = node.ownerDocument || node;
            var _queryString = queryString + String.Empty;

            if (doc.evaluate) {
                var xPathResult = doc.evaluate(_queryString, node, !namespaces ? null : function (prefix) {
                    return (namespaces[prefix] || String.Empty) + String.Empty;
                }, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
                result = xPathResult.singleNodeValue;
            } else {
                var ieNamespaces = String.Empty;
                if (namespaces) {
                    for (prefix in namespaces) {
                        prefix && namespaces[prefix] && (ieNamespaces += "xmlns:" + prefix + "='" + namespaces[prefix] + "' ");
                    }
                }
                ieNamespaces = ieNamespaces.TrimEnd();

                doc.setProperty("SelectionLanguage", "XPath");
                doc.setProperty("SelectionNamespaces", ieNamespaces);
                result = (doc == node ? doc.documentElement : node).selectSingleNode(_queryString);
            }

            return result;
        },

        /*
        * Method Description:
        * Executes a XPath query and returns the match nodes array.
        * 
        * Parameters:
        * node: The executing context of XPath query.
        * queryString : A XPath query string;
        * namespaces: A object. The name of a property in this object is a XML namespace prefix and it's value is a XML namespace URL.
        *
        * Returns:
        * Return the match nodes array.
        */
        SelectNodes: function (node, queryString, namespaces) {
            if (!node) {
                throw new Error("Node is undefined.");
            }
            if (!queryString) {
                throw new Error("XPath query string is undefined.");
            }

            var result = [];
            var doc = node.ownerDocument || node;
            var _queryString = queryString + String.Empty;

            if (doc.evaluate) {
                var xPathResult = doc.evaluate(_queryString, node, !namespaces ? null : function (prefix) {
                    return (namespaces[prefix] || String.Empty) + String.Empty;
                }, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
                for (var i = 0; i < xPathResult.snapshotLength; i++) {
                    result.push(xPathResult.snapshotItem(i));
                }
            } else {
                var ieNamespaces = String.Empty;
                if (namespaces) {
                    for (prefix in namespaces) {
                        prefix && namespaces[prefix] && (ieNamespaces += "xmlns:" + prefix + "=\"" + namespaces[prefix] + "\" ");
                    }
                }
                ieNamespaces = ieNamespaces.TrimEnd();

                doc.setProperty("SelectionLanguage", "XPath");
                doc.setProperty("SelectionNamespaces", ieNamespaces);
                var nodes = (doc == node ? doc.documentElement : node).selectNodes(_queryString);
                for (var i = 0; i < nodes.length; i++) {
                    result.push(nodes[i]);
                }
            }

            return result;
        }
    };

    /***************** XmlSerializer Class Definition *****************/
    /*
    * Class Description:
    * Provides a utility for serialize object to XML document.
    *
    * Mask:
    * This class don't serialize function and array property of a object.
    */
    var XmlSerializer = Xphter.XmlSerializer = function () {
        /*
        * Method Description:
        * Serializes the specified object to a XML node.
        *
        * Parameters:
        * xmlDocument: A XML document.
        * obj: A object, can be a number, string, boolean and object.
        * objectName: The tag name, can contains a namespace prefix.
        * namespaceURL: The node namespace.
        *
        * Return:
        * Returns a XML node represents the specified object.
        */
        this.SerializeToXmlNode = function (xmlDocument, obj, objectName, namespaceURL) {
            if (!xmlDocument) {
                throw new Error("XML document is undefined.");
            }
            if (DataTypeIdentity.IsUndefinedOrNull(obj)) {
                throw new Error("Object is undefined.");
            }
            if (!objectName) {
                throw new Error("Object name is undefined.");
            }

            var node = null;
            var _objectName = objectName + String.Empty;
            switch (typeof obj) {
                case "number":
                case "string":
                case "boolean":
                    node = XmlUtility.CreateElementNS(xmlDocument, _objectName, namespaceURL);
                    node.appendChild(xmlDocument.createTextNode(obj + String.Empty));
                    break;
                case "object":
                    if (!DataTypeIdentity.IsArray(obj)) {
                        var childNode = null;
                        var match = (/^([^\:]+)\:/i).exec(_objectName);
                        var prefix = match ? match[1] : null;
                        node = XmlUtility.CreateElementNS(xmlDocument, _objectName, namespaceURL);
                        for (var name in obj) {
                            if (name && (childNode = this.SerializeToXmlNode(xmlDocument, obj[name], (prefix ? prefix + ":" : String.Empty) + name, xmlDocument.createElementNS ? namespaceURL : null))) {
                                node.appendChild(childNode);
                            }
                        }
                    }
                    break;
            }

            return node;
        };

        /*
        * Method Description:
        * Serializes the specified object.
        *
        * Parameters:
        * obj: A object, can be a number, string, boolean and object.
        * objectName: The tag name of root XML element.
        * namespaceURL: The node namespace.
        *
        * Return:
        * Returns a XML document represents the specified object.
        */
        this.Serialize = function (obj, objectName, namespaceURL) {
            if (DataTypeIdentity.IsUndefinedOrNull(obj)) {
                throw new Error("Object is undefined.");
            }
            if (DataTypeIdentity.IsUndefinedOrNull(objectName)) {
                throw new Error("Object name is undefined.");
            }
            switch (typeof obj) {
                case "function":
                    throw new Error("Can not serialize function.");
                    break;
                case "object":
                    if (DataTypeIdentity.IsArray(obj)) {
                        throw new Error("Can not serialize array.");
                    }
                    break;
            }

            var xmlDocument = XmlUtility.CreateDocument();
            xmlDocument.appendChild(this.SerializeToXmlNode(xmlDocument, obj, objectName + String.Empty, namespaceURL));
            return xmlDocument;
        }
    };

    /***************** WebService Class Definition *****************/
    /*
    * Class Description:
    * Provides a utility for calling a XML web service.
    *
    * Constructor Parameters:
    * url : The URL of this webservice;
    * encoding: The encoding of page when using SOAP or encoding of response data when cross-domain call.
    */
    var WebService = Xphter.WebService = function (url, encoding) {
        var m_url = null;
        var m_encoding = null;
        var m_namespace = null;
        var m_soap = null;
        var m_isCrossDomain;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!url) {
                throw new Error("Web service URL string is undefined.");
            }
            var uri = new Uri(m_url = url + String.Empty);
            m_encoding = (encoding || "utf-8") + String.Empty;
            if (!uri.GetIsWellFormat()) {
                throw new Error("Invalid web service URL.");
            }
            m_isCrossDomain = !uri.GetIsRelative() && !uri.GetHost().EqualsIgnoreCase(global.location.host);
            m_soap = "<?xml version=\"1.0\" encoding=\"" + m_encoding + "\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body></soap12:Body></soap12:Envelope>";
        }

        /*
        * Private Method Description:
        * Parse the specified argument object to query string.
        */
        function ArgumentsToQueryString(args) {
            if (!args) {
                return null;
            }

            var value = null;
            var queryString = String.Empty;

            for (var name in args) {
                if (!name || DataTypeIdentity.IsUndefinedOrNull(value = args[name])) {
                    continue;
                }

                queryString += ((queryString.length > 0 ? "&" : String.Empty) + global.encodeURIComponent(name) + "=" + global.encodeURIComponent(value + String.Empty));
            }

            return queryString;
        }

        /*
        * Private Method Description:
        * Parse the specified argument object to a SOAP XML document.
        */
        function ArgumentsToSoapDocument(args, methodName) {
            var doc = XmlUtility.LoadString(m_soap);
            var node = new XmlSerializer().SerializeToXmlNode(doc, args || {}, methodName, m_namespace || (m_namespace = GetNamespace()));
            doc.documentElement.firstChild.appendChild(node);
            return doc;
        }

        /*
        * Private Method Description:
        * Gets default namespace of this webservice.
        */
        function GetNamespace() {
            if (m_isCrossDomain) {
                throw new Error("Can not get namespace of a webservice in a cross domain.");
            }

            var wsdl = XmlUtility.LoadFile(m_url + "?WSDL");
            return XmlUtility.SelectSingleNode(wsdl, "//@targetNamespace").value;
        }

        /*
        * Method Description:
        * Gets whether the specified web service is location in a different domain.
        */
        this.GetIsCrossDomain = function () {
            return m_isCrossDomain;
        };

        /*
        * Method Description:
        * Synchronous invoke this webservice, a error will throw if this is a cross-domain invoke.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   MethodName: The name of a webservice method.
        *   Method: HTTP GET or HTTP POST or SOAP.
        *   Arguments: The arguments.
        *   OnSuccess: The callback function when successfully load resource. The response object or a XMLHttpRequest object argument will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnComplete: The callback function when complete load resource.
        * }
        *
        */
        this.SyncCall = function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }
            if (!parameters.MethodName) {
                throw new Error("Method name is empty.");
            }
            if (m_isCrossDomain) {
                throw new Error("This is cross domain invoke, please call CrossDomainCall method instend.");
            }

            switch (((parameters.Method || "soap") + String.Empty).toLowerCase()) {
                case "get":
                    AjaxUtility.Sync({
                        Uri: m_url + "/" + parameters.MethodName + (parameters.Arguments ? "?" + ArgumentsToQueryString(parameters.Arguments) : String.Empty),
                        Method: HttpMethods.Get,
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnComplete: parameters.OnComplete
                    });
                    break;
                case "post":
                    AjaxUtility.Sync({
                        Uri: m_url + "/" + parameters.MethodName,
                        Method: HttpMethods.Post,
                        Data: ArgumentsToQueryString(parameters.Arguments),
                        Header: {
                            "Content-Type": "application/x-www-form-urlencoded; charset=" + m_encoding
                        },
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnComplete: parameters.OnComplete
                    });
                    break;
                default:
                    AjaxUtility.Sync({
                        Uri: m_url,
                        Method: HttpMethods.Post,
                        Data: ArgumentsToSoapDocument(parameters.Arguments, parameters.MethodName),
                        Header: {
                            "Content-Type": "application/soap+xml; charset=" + m_encoding
                        },
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnComplete: parameters.OnComplete
                    });
                    break;
            }
        };

        /*
        * Method Description:
        * Asynchronous invoke this webservice, a error will throw if this is a cross-domain invoke.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   MethodName: The name of a webservice method.
        *   Method: HTTP GET or HTTP POST or SOAP.
        *   Arguments: The arguments.
        *   OnSuccess: The callback function when successfully load resource. The response object or a XMLHttpRequest object argument will pass to this method.
        *   OnError: The callback function when unsuccessfully load resource. A XMLHttpRequest object argument will pass to this method.
        *   OnTimeout: The callback function when load resource timeout.
        *   OnComplete: The callback function when complete load resource.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        this.AsyncCall = function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }
            if (!parameters.MethodName) {
                throw new Error("Method name is empty.");
            }
            if (m_isCrossDomain) {
                throw new Error("This is cross domain invoke, please call CrossDomainCall method instend.");
            }

            switch (((parameters.Method || "soap") + String.Empty).toLowerCase()) {
                case "get":
                    AjaxUtility.Async({
                        Uri: m_url + "/" + parameters.MethodName + (parameters.Arguments ? "?" + ArgumentsToQueryString(parameters.Arguments) : String.Empty),
                        Method: HttpMethods.Get,
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnTimeout: parameters.OnTimeout,
                        OnComplete: parameters.OnComplete,
                        Timeout: parameters.Timeout
                    });
                    break;
                case "post":
                    AjaxUtility.Async({
                        Uri: m_url + "/" + parameters.MethodName,
                        Method: HttpMethods.Post,
                        Data: ArgumentsToQueryString(parameters.Arguments),
                        Header: {
                            "Content-Type": "application/x-www-form-urlencoded; charset=" + m_encoding
                        },
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnTimeout: parameters.OnTimeout,
                        OnComplete: parameters.OnComplete,
                        Timeout: parameters.Timeout
                    });
                    break;
                default:
                    AjaxUtility.Async({
                        Uri: m_url,
                        Method: HttpMethods.Post,
                        Data: ArgumentsToSoapDocument(parameters.Arguments, parameters.MethodName),
                        Header: {
                            "Content-Type": "application/soap+xml; charset=" + m_encoding
                        },
                        OnSuccess: parameters.OnSuccess,
                        OnError: parameters.OnError,
                        OnTimeout: parameters.OnTimeout,
                        OnComplete: parameters.OnComplete,
                        Timeout: parameters.Timeout
                    });
                    break;
            }
        };

        /*
        * Method Description:
        * Cross domain invoke this webservice.
        *
        * Parameters:
        * parameters: The parameters for this calling which is a object as follow:
        * {
        *   MethodName: The name of a webservice method.
        *   Arguments: The arguments.
        *   OnComplete: The callback function when successfully load resource..
        *   OnTimeout: The callback function when load resource timeout.
        *   Timeout: The timeout in milliseconeds. Zero or a negative number indicates infinity timeout.
        * }
        *
        */
        this.CrossDomainCall = function (parameters) {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters is undefined.");
            }
            if (!parameters.MethodName) {
                throw new Error("Method name is empty.");
            }

            AjaxUtility.CrossDomain({
                Uri: m_url + "/" + parameters.MethodName + (parameters.Arguments ? "?" + ArgumentsToQueryString(parameters.Arguments) : String.Empty),
                Charset: m_encoding,
                OnComplete: parameters.OnComplete,
                OnTimeout: parameters.OnTimeout,
                Timeout: parameters.Timeout
            });
        };
    };

    /***************** Animator Class Definition *****************/

    /*
    * Class Description:
    * Provides base functions of animation.
    *
    * Constructor Parameters:
    * parameters: The parameters for this calling which is a object as follow:
    * {
    *   Fps: The fps which should less than or equal 1000 and greater than or equal 0.
    *   Duration: The all milliseconds begin start animation to end animation, set zero to indicate never automatic stop animation until invoke Stop method explicitly.
    *   OnInitialize: The initialize operation before execute the first frame.
    *   OnFrame: The animation operation, the elapsed time will pass to this function.
    *   OnDispose: The dispose operation after execute the last frame, the elapsed time will pass to this function.
    * }
    */
    var Animator = Xphter.Animator = function (parameters) {
        var m_fps = 0;
        var m_duration = 0;
        var m_onInitialize = null;
        var m_onFrame = null;
        var m_onDispose = null;

        var m_currentFrameIndex = 0;
        var m_isRunning = false;
        var m_isPaused = false;

        var m_timer = null;
        var m_interval = 0;
        var m_baseTick = 0;
        var m_baseElapsedTime = 0;
        var m_elapsedTime = 0;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            m_fps = Math.min(1000, Math.max(1, !global.isNaN(parameters.Fps) ? Math.floor(parameters.Fps - 0) : 0));
            m_duration = Math.max(0, !global.isNaN(parameters.Duration) ? Math.floor(parameters.Duration - 0) : 0);
            m_onInitialize = DataTypeIdentity.IsFunction(parameters.OnInitialize) ? parameters.OnInitialize : null;
            m_onFrame = DataTypeIdentity.IsFunction(parameters.OnFrame) ? parameters.OnFrame : null;
            m_onDispose = DataTypeIdentity.IsFunction(parameters.OnDispose) ? parameters.OnDispose : null;
            m_interval = Math.round(1000 / m_fps);
        }

        /*
        * Private Method Description:
        * Start animation, do nothing if this animation has already started.
        */
        function StartAnimator() {
            if (m_isRunning) {
                return;
            }

            //initialize
            m_isRunning = true;
            m_isPaused = false;
            m_currentFrameIndex = 0;
            m_baseTick = new Date().getTime();
            m_baseElapsedTime = 0;
            m_elapsedTime = 0;

            //perform initialization
            m_onInitialize && m_onInitialize();

            //first frame
            m_onFrame && m_onFrame(m_elapsedTime);

            //start timer
            if (!m_duration) {
                m_timer = global.setInterval(function () {
                    if (m_isPaused) {
                        m_baseTick = new Date().getTime();
                        m_baseElapsedTime = m_elapsedTime;
                        return;
                    }

                    //each frame
                    ++m_currentFrameIndex;
                    m_onFrame && m_onFrame(m_elapsedTime = new Date().getTime() - m_baseTick + m_baseElapsedTime);
                }, m_interval);
            } else {
                m_timer = global.setInterval(function () {
                    if (m_isPaused) {
                        m_baseTick = new Date().getTime();
                        m_baseElapsedTime = m_elapsedTime;
                        return;
                    }

                    //each frame
                    ++m_currentFrameIndex;
                    m_onFrame && m_onFrame(m_elapsedTime = new Date().getTime() - m_baseTick + m_baseElapsedTime);

                    //check whether the duration is expired.
                    (m_elapsedTime >= m_duration) && StopAnimator();
                }, m_interval);
            }
        }

        /*
        * Private Method Description:
        * Stop animation, do nothing if this animation has already stoped.
        */
        function StopAnimator() {
            if (!m_isRunning) {
                return;
            }

            //initialize
            m_isRunning = false;

            //close timer
            global.clearInterval(m_timer);
            m_timer = null;

            //perform dispose
            m_onDispose && m_onDispose(m_elapsedTime);
        }

        /*
        * Private Method Description:
        * Pause or resume animation, do nothing if this animation is not running.
        */
        function PauseAnimator(paused) {
            if (!m_isRunning) {
                return;
            }

            m_isPaused = !!paused;
        }

        /*
        * Method Description:
        * Gets current frame index numbering from zero.
        */
        this.GetCurrentFrameIndex = function () {
            return m_currentFrameIndex;
        };

        /*
        * Method Description:
        * Gets whether the animation is running.
        */
        this.GetIsRunning = function () {
            return m_isRunning;
        };

        /*
        * Method Description:
        * Gets whether the animation is paused.
        */
        this.GetIsPaused = function () {
            return m_isPaused;
        };

        /*
        * Method Description:
        * Gets whether the animation will never automatic stop until invoke Stop method explicitly.
        */
        this.GetIsForever = function () {
            return !m_duration;
        };

        /*
        * Method Description:
        * Gets elapsed milliseconds.
        */
        this.GetElapsedTime = function () {
            return m_elapsedTime;
        };

        /*
        * Method Description:
        * Start animation, do nothing if this animation has already started.
        */
        this.Start = StartAnimator;

        /*
        * Method Description:
        * Pause animation, do nothing if this animation has already paused.
        */
        this.Pause = function () {
            PauseAnimator(true);
        };

        /*
        * Method Description:
        * Resume animation, do nothing if this animation did not paused.
        */
        this.Resume = function () {
            PauseAnimator(false);
        };

        /*
        * Method Description:
        * Stop animation, do nothing if this animation has already stoped.
        */
        this.Stop = StopAnimator;
    };

    /***************** Gallery Class Definition *****************/

    /*
    * Class Description:
    * Represents a sliding gallery.
    * In a sliding gallery, it's child elements will slide from bottom to top or slide from left to right.
    * The sliding direction lies on user choice.
    *
    * Constructor Parameters:
    * parameters: The parameters for this calling which is a object as follow:
    * {
    *   Target: The element will be a sliding gallery.
    *   Length: The element number in this gallery. The value is effective only when vertical sliding.
    *   Direction: The sliding direction which should be one of SlidingDirection's member.
    *   Fps: The frame rate.
    *   Speed: The pixels number per seconds when sliding.
    *   IsPauseOnMouseEnter: Whether pause this gallery when mouse enter the target element.
    * }
    */
    var Gallery = Xphter.Gallery = function (parameters) {
        var m_target = null;
        var m_length = 0;
        var m_direction = Gallery.SlidingDirection.BottomToTop;
        var m_fps = 0;
        var m_isPauseOnMouseEnter = true;

        var m_childNodes = null;
        var m_referenceElement = null;
        var m_originalStart = 0;
        var m_originalEnd = 0;
        var m_isCircling = false;

        var m_speed = 0;
        var m_offset = 0;
        var m_animator = null;
        var m_elapsedTime = 0;

        Constructor();

        /*
        * Constructor
        */
        function Constructor() {
            m_target = parameters.Target;
            !global.isNaN(parameters.Length) && (m_length = parameters.Length - 0);
            !global.isNaN(parameters.Direction) && (m_direction = parameters.Direction - 0);
            m_fps = Math.max(1, !global.isNaN(parameters.Fps) ? Math.floor(parameters.Fps - 0) : 0);
            m_speed = Math.max(1, !global.isNaN(parameters.Speed) ? Math.floor(parameters.Speed - 0) : 0);
            !DataTypeIdentity.IsUndefinedOrNull(parameters.IsPauseOnMouseEnter) && (m_isPauseOnMouseEnter = !!parameters.IsPauseOnMouseEnter);

            if (!DataTypeIdentity.IsNotNullObject(m_target)) {
                throw new Error("The target element of gallery is undefined.");
            }

            //initialize container
            m_target.style.overflow = "hidden";
            switch (m_direction) {
                case Gallery.SlidingDirection.LeftToRight:
                case Gallery.SlidingDirection.RightToLeft:
                    m_target.style.whiteSpace = "nowrap";
                    break;
            }
            if (!m_target.style.position || m_target.style.position.EqualsIgnoreCase("static")) {
                m_target.style.position = "relative";
                m_target.style.left = "0px";
                m_target.style.top = "0px";
            }

            //search element child nodes
            var node = null;
            var children = [];
            while (node = m_target.firstChild) {
                if (node.nodeType == DomNodeTypes.Element) {
                    //set style
                    node.style.position = "relative";
                    node.style.left = "0px";
                    node.style.top = "0px";

                    //save element node
                    children.push(node);
                }

                m_target.removeChild(node);
            }

            for (var i = 0; i < children.length; i++) {
                m_target.appendChild(children[i]);
            }

            m_childNodes = m_target.childNodes;
            m_length = m_length <= 0 ? children.length : Math.min(m_length, children.length);
            m_referenceElement = m_length == children.length ? null : children[m_length];

            //get start and end of elements
            if (m_childNodes.length > 0) {
                var firstChild = m_childNodes[0];
                var lastChild = m_childNodes[m_childNodes.length - 1];
                switch (m_direction) {
                    case Gallery.SlidingDirection.BottomToTop:
                    case Gallery.SlidingDirection.TopToBottom:
                        m_originalStart = firstChild.offsetTop;
                        m_originalEnd = (lastChild.offsetTop + lastChild.offsetHeight);
                        m_isCircling = (m_originalEnd - m_originalStart) > m_target.offsetHeight && m_childNodes.length > 1;
                        break;
                    case Gallery.SlidingDirection.LeftToRight:
                    case Gallery.SlidingDirection.RightToLeft:
                        m_originalStart = firstChild.offsetLeft;
                        m_originalEnd = (lastChild.offsetLeft + lastChild.offsetWidth);
                        m_isCircling = (m_originalEnd - m_originalStart) > m_target.offsetWidth && m_childNodes.length > 1;
                        break;
                }
            }

            //register event handler for mouse moving
            EventUtility.Register(m_target, "mouseover", function () {
                m_isPauseOnMouseEnter && m_animator && m_animator.Pause();
            }, false);
            EventUtility.Register(m_target, "mouseout", function () {
                m_isPauseOnMouseEnter && m_animator && m_animator.Resume();
            }, false);
        }

        /*
        * Private Method Description:
        * Sliding bottom to top.
        */
        function BottomToTop(elapsedTime) {
            var delta = (elapsedTime - m_elapsedTime) * m_speed / 1000;
            if (m_isCircling) {
                var firstChild = m_childNodes[0];
                var secondChild = m_childNodes[1];
                var secondOffsetTop = secondChild.offsetTop;
                if (Math.abs(m_offset - delta) >= DocumentUtility.GetElementHeight(firstChild)) {
                    m_target.insertBefore(firstChild, m_referenceElement);

                    m_offset -= (secondChild.offsetTop - secondOffsetTop);
                } else {
                    m_offset -= delta;
                }
            } else {
                var lastChild = m_childNodes[m_childNodes.length - 1];
                if (lastChild.offsetTop + lastChild.offsetHeight - delta < 0) {
                    m_offset = (m_target.offsetHeight - m_originalStart);
                } else {
                    m_offset -= delta;
                }
            }

            for (var i = 0; i < m_childNodes.length; i++) {
                m_childNodes[i].style.top = Math.floor(m_offset) + "px";
            }

            m_elapsedTime = elapsedTime;
        }

        /*
        * Private Method Description:
        * Sliding top to bottom.
        */
        function TopToBottom(elapsedTime) {
            var delta = (elapsedTime - m_elapsedTime) * m_speed / 1000;
            if (m_isCircling) {
                var firstChild = m_childNodes[0];
                var firstOffsetTop = firstChild.offsetTop;
                var lastChild = m_childNodes[m_length - 1];
                if (m_offset + delta >= 0) {
                    m_target.insertBefore(lastChild, firstChild);

                    m_offset -= (firstChild.offsetTop - firstOffsetTop);
                } else {
                    m_offset += delta;
                }
            } else {
                var firstChild = m_childNodes[0];
                if (firstChild.offsetTop + delta > m_target.offsetHeight) {
                    m_offset = (0 - m_originalEnd);
                } else {
                    m_offset += delta;
                }
            }

            for (var i = 0; i < m_childNodes.length; i++) {
                m_childNodes[i].style.top = Math.floor(m_offset) + "px";
            }

            m_elapsedTime = elapsedTime;
        }

        /*
        * Private Method Description:
        * Sliding left to right.
        */
        function LeftToRight(elapsedTime) {
            var delta = (elapsedTime - m_elapsedTime) * m_speed / 1000;
            if (m_isCircling) {
                var firstChild = m_childNodes[0];
                var firstOffsetLeft = firstChild.offsetLeft;
                var lastChild = m_childNodes[m_length - 1];
                if (m_offset + delta >= 0) {
                    m_target.insertBefore(lastChild, firstChild);

                    m_offset -= (firstChild.offsetLeft - firstOffsetLeft);
                } else {
                    m_offset += delta;
                }
            } else {
                var firstChild = m_childNodes[0];
                if (firstChild.offsetLeft + delta > m_target.offsetWidth) {
                    m_offset = (0 - m_originalEnd);
                } else {
                    m_offset += delta;
                }
            }

            for (var i = 0; i < m_childNodes.length; i++) {
                m_childNodes[i].style.left = Math.floor(m_offset) + "px";
            }

            m_elapsedTime = elapsedTime;
        }

        /*
        * Private Method Description:
        * Sliding right to left.
        */
        function RightToLeft(elapsedTime) {
            var delta = (elapsedTime - m_elapsedTime) * m_speed / 1000;
            if (m_isCircling) {
                var firstChild = m_childNodes[0];
                var secondChild = m_childNodes[1];
                var secondOffsetLeft = secondChild.offsetLeft;
                if (Math.abs((m_offset - delta)) >= DocumentUtility.GetElementWidth(firstChild)) {
                    m_target.insertBefore(firstChild, m_referenceElement);

                    m_offset -= (secondChild.offsetLeft - secondOffsetLeft);
                } else {
                    m_offset -= delta;
                }
            } else {
                var lastChild = m_childNodes[m_childNodes.length - 1];
                if (lastChild.offsetLeft + lastChild.offsetWidth - delta < 0) {
                    m_offset = m_target.offsetWidth - m_originalStart;
                } else {
                    m_offset -= delta;
                }
            }

            for (var i = 0; i < m_childNodes.length; i++) {
                m_childNodes[i].style.left = Math.floor(m_offset) + "px";
            }

            m_elapsedTime = elapsedTime;
        }

        /*
        * Method Description:
        * Gets whether the sliding gallery is running.
        */
        this.GetIsRunning = function () {
            return !!m_animator;
        };

        /*
        * Method Description:
        * Gets whether the sliding gallery has paused.
        */
        this.GetIsPaused = function () {
            return !!(m_animator && m_animator.GetIsPaused());
        };

        /*
        * Method Description:
        * Gets whether pause this sliding gallery when mouse enter the target element.
        *
        * Return:
        * Return whether pause this sliding gallery when mouse enter the target element.
        */
        this.GetIsPauseOnMouseEnter = function () {
            return m_isPauseOnMouseEnter;
        };

        /*
        * Method Description:
        * Sets whether pause this sliding gallery when mouse enter the target element.
        *
        * Parameters:
        * value: whether pause this sliding gallery when mouse enter the target element.
        */
        this.SetIsPauseOnMouseEnter = function (value) {
            m_isPauseOnMouseEnter = !!value;
        };

        /*
        * Method Description:
        * Gets speed of this gallery: pixels per second when moving.
        */
        this.GetSpeed = function () {
            return m_speed;
        };

        /*
        * Method Description:
        * Sets speed of this gallery: pixels per second when moving.
        *
        * Parameters:
        * value: The new value of speed.
        */
        this.SetSpeed = function (value) {
            m_speed = Math.max(1, !global.isNaN(value) ? Math.floor(value - 0) : 0);
        };

        /*
        * Method Description:
        * Change the speed.
        *
        * Parameter:
        * offset: The value will changed. If it is a positive number, this gallery will speedup, otherwise it will speeddown.
        */
        this.ChangeSpeed = function (offset) {
            var _offset = !global.isNaN(offset) ? Math.floor(offset - 0) : 0;
            m_speed = Math.max(1, m_speed + _offset);
        };

        /*
        * Method Description:
        * Start to run this sliding gallery.
        */
        this.Start = function () {
            if (m_animator || m_childNodes.length < 1) {
                return;
            }

            m_elapsedTime = 0;
            switch (m_direction) {
                case Gallery.SlidingDirection.BottomToTop:
                    m_animator = new Animator({
                        Fps: m_fps,
                        OnFrame: BottomToTop
                    });
                    break;
                case Gallery.SlidingDirection.TopToBottom:
                    m_animator = new Animator({
                        Fps: m_fps,
                        OnFrame: TopToBottom
                    });
                    break;
                case Gallery.SlidingDirection.LeftToRight:
                    m_animator = new Animator({
                        Fps: m_fps,
                        OnFrame: LeftToRight
                    });
                    break;
                case Gallery.SlidingDirection.RightToLeft:
                    m_animator = new Animator({
                        Fps: m_fps,
                        OnFrame: RightToLeft
                    });
                    break;
            }
            m_animator.Start();
        };

        /*
        * Method Description:
        * Stop runing this sliding gallery.
        */
        this.Stop = function () {
            if (!m_animator) {
                return;
            }

            m_animator.Stop();
            m_animator = null;
        };

        /*
        * Method Description:
        * Puase this sliding gallery.
        */
        this.Pause = function () {
            m_animator && m_animator.Pause();
        };

        /*
        * Method Description:
        * Continue to run this sliding gallery if it has paused.
        */
        this.Resume = function () {
            m_animator && m_animator.Resume();
        };
    };

    /*
    * Enumrator Description:
    * Represents the direction of a sliding gallery.
    */
    Gallery.SlidingDirection = {
        BottomToTop: 0,
        TopToBottom: 1,
        LeftToRight: 2,
        RightToLeft: 3
    };

    /***************** Flash Class Definition *****************/

    /*
    * Class Description:
    * Represents a flash control.
    *
    * Constructor Parameters:
    * parameters: The parameters for this calling which is a object as follow:
    * {
    *   Uri: The URI of a flash file.
    *   Width: The width of this flash. It can be a number of a string, such as "100%".
    *   Height: The height of this flash. It can be a number of a string, such as "100%".
    *   Styles: The CSS style of the flash element.
    *   Properties: A object which's enumerable property will be a parameter of this flash control.
    * }
    */
    var Flash = Xphter.Flash = function (parameters) {
        var m_uri = String.Empty;
        var m_width = null;
        var m_widthProperty = String.Empty;
        var m_height = null;
        var m_heightProperty = String.Empty;
        var m_styles = null;
        var m_properties = null;
        var m_element = null;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            m_uri = parameters.Uri + String.Empty;
            m_width = parameters.Width;
            m_widthProperty = DataTypeIdentity.IsUndefinedOrNull(m_width) ? String.Empty : (!global.isNaN(m_width) ? Math.floor(m_width - 0) + "px" : m_width + String.Empty);
            m_height = parameters.Height;
            m_heightProperty = DataTypeIdentity.IsUndefinedOrNull(m_height) ? String.Empty : (!global.isNaN(m_height) ? Math.floor(m_height - 0) + "px" : m_height + String.Empty);
            m_styles = parameters.Styles;
            m_properties = parameters.Properties || {};

            //create object markup
            /*var objectParameters = [];
            if (parameters) {
            for (var name in parameters) {
            objectParameters.push(CreateParam(name, parameters[name]));
            }
            }
            objectParameters.push(CreateParam("movie", m_uri));
            objectParameters.push(CreateParam("src", m_uri));
            var obj = DocumentUtility.CreateElement("object", {
            type: "application/x-shockwave-flash",
            classid: "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
            codebase: "http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab",
            data: m_flash,
            width: m_width,
            height: m_height
            }, styles, objectParameters);*/

            //create embed markup
            var embedParameters = m_properties;
            embedParameters["type"] = "application/x-shockwave-flash";
            embedParameters["pluginspage"] = "http://www.macromedia.com/go/getflashplayer";
            embedParameters["src"] = m_uri;
            embedParameters["width"] = m_widthProperty;
            embedParameters["height"] = m_heightProperty;
            var embed = DocumentUtility.CreateElement("embed", embedParameters, m_styles);

            m_element = embed;
        }

        /*
        * Private Method Description:
        * Create a param markup for object markup.
        */
        function CreateParam(name, value) {
            var param = global.document.createElement("param");
            param.setAttribute("name", name + "");
            param.setAttribute("value", value + "");
            return param;
        }

        /*
        * Method Description:
        * Gets the width of flash.
        */
        this.GetWidth = function () {
            return m_width;
        };

        /*
        * Method Description:
        * Gets the height of flash.
        */
        this.GetHeight = function () {
            return m_height;
        };

        /*
        * Method Description:
        * Gets the flash HTML element.
        */
        this.GetElement = function () {
            return m_element;
        };

        /*
        * Method Description:
        * Replaces a existing element with this flash control.
        * 
        * Parameters:
        * element: The element will be replaced.
        */
        this.Replace = function (element) {
            element && element.parentNode && element.parentNode.replaceChild(m_element, element);
        };

        /*
        * Method Description:
        * Places the flash control in the specified element.
        * 
        * Parameters:
        * element: The element will contain this flash control.
        */
        this.Place = function (element) {
            element && element.appendChild && element.appendChild(m_element);
        };
    };

    /***************** FlashAnchor Class Definition *****************/

    /*
    * Class Description:
    * Represents a flash anchor.
    *
    * Constructor Parameters:
    * parameters: The parameters for this calling which is a object as follow:
    * {
    *   Uri: The URI of a flash file.
    *   Href: The navigated uri when user click this flash.
    *   Width: The width of this flash. It can be a number of a string, such as "100%".
    *   Height: The height of this flash. It can be a number of a string, such as "100%".
    *   Properties: A object which's enumerable property will be a parameter of this flash control.
    *   OnClick: The handler for click event.
    * }
    */
    var FlashAnchor = Xphter.FlashAnchor = function (parameters) {
        var m_uri = null;
        var m_href = null;
        var m_width = null;
        var m_widthProperty = String.Empty;
        var m_height = null;
        var m_heightProperty = String.Empty;
        var m_properties = null;
        var m_onClick = null;
        var m_element = null;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            m_uri = parameters.Uri + String.Empty;
            !DataTypeIdentity.IsUndefinedOrNull(parameters.Href) && (m_href = parameters.Href + String.Empty);
            m_width = parameters.Width;
            m_widthProperty = !global.isNaN(m_width) ? Math.floor(m_width - 0) + "px" : m_width + String.Empty;
            m_height = parameters.Height;
            m_heightProperty = !global.isNaN(m_height) ? Math.floor(m_height - 0) + "px" : m_height + String.Empty;
            m_properties = parameters.Properties || {};
            m_onClick = DataTypeIdentity.IsFunction(parameters.OnClick) ? parameters.OnClick : function () { };

            //create flash layer
            var flashLayer = DocumentUtility.CreateElement("div", null, {
                width: "100%",
                height: "100%",
                position: "absolute",
                left: "0px",
                top: "0px",
                margin: "0px",
                padding: "0px"
            });

            //create flash element
            m_properties["wmode"] = "opaque";
            var flash = new Flash({
                Uri: m_uri,
                Width: "100%",
                Height: "100%",
                Properties: m_properties
            });
            flash.Place(flashLayer);

            //create anchor layer
            var anchorLayer = DocumentUtility.CreateElement("div", null, {
                width: "100%",
                height: "100%",
                position: "absolute",
                left: "0px",
                top: "0px",
                margin: "0px",
                padding: "0px",
                zIndex: 99,
                cursor: "pointer",
                backgroundImage: "url('about:blank')"
            });

            //create anchor element
            var anchor = m_href ? DocumentUtility.CreateElement("a", {
                href: m_href,
                target: "_blank"
            }, {
                display: "block",
                width: "100%",
                height: "100%",
                margin: "0px",
                padding: "0px"
            }) : null;
            anchor && anchorLayer.appendChild(anchor);

            if (anchor) {
                EventUtility.Register(anchor, "click", m_onClick, false);
            } else {
                EventUtility.Register(anchorLayer, "click", m_onClick, false);
            }

            //create anchor element
            m_element = DocumentUtility.CreateElement("div", null, {
                width: m_widthProperty,
                height: m_heightProperty,
                position: "relative",
                left: "0px",
                top: "0px",
                margin: "0px",
                padding: "0px"
            }, [flashLayer, anchorLayer]);
        }

        /*
        * Method Description:
        * Gets the width of flash.
        */
        this.GetWidth = function () {
            return m_width;
        };

        /*
        * Method Description:
        * Gets the height of flash.
        */
        this.GetHeight = function () {
            return m_height;
        };

        /*
        * Method Description:
        * Gets the anchor HTML element.
        */
        this.GetElement = function () {
            return m_element;
        };

        /*
        * Method Description:
        * Replaces a existing element with this flash anchor.
        * 
        * Parameters:
        * element: The element will be replaced.
        */
        this.Replace = function (element) {
            element && element.parentNode && element.parentNode.replaceChild(m_element, element);
        };

        /*
        * Method Description:
        * Places the flash anchor in the specified element.
        * 
        * Parameters:
        * element: The element will contain this flash anchor control.
        */
        this.Place = function (element) {
            element && element.appendChild && element.appendChild(m_element);
        };
    };

    /***************** Couplet Class Definition *****************/

    /*
    * Class Description:
    * Represents a couplet.
    *
    * Constructor Parameters:
    * parameters: The parameters for creating this couplet. It is a object as follow:
    * {
    *   IsFixed: Determine whether fix the position of this couplet.
    *   SlidingDuration: The duration miliseconds when sliding this couplet if position of it is fixed.
    *   Top: The top position of this couplet.
    *   SmallWidth: The width of couplet when it has collapsed.
    *   LargeWidth: The width of couplet when it has expanded.
    *   Height: The height of couplet.
    *   LeftHref: The navigated URI when user click left media.
    *   OnLeftClick: The callback operation when user click left media.
    *   LeftSmallUri: The URI of left file when couplet has collapsed.
    *   IsLeftSmallImage: Indicate whether the LeftSmallUri represents a image.
    *   LeftLargeUri: The URI of left file when couplet has expanded.
    *   IsLeftLargeImage: Indicate whether the LeftLargeUri represents a image.
    *   RightHref: The navigated URI when user click right media.
    *   OnLeftClick: The callback operation when user click left media.
    *   RightSmallUri: The URI of right file when couplet has collapsed.
    *   IsRightSmallImage: Indicate whether the RightSmallUri represents a image.
    *   RightLargeUri: The URI of right file when couplet has expanded.
    *   IsRightLargeImage: Indicate whether the RightLargeUri represents a image.
    * }
    */
    var Couplet = Xphter.Couplet = function (parameters) {
        var m_isFixed = false;
        var m_slidingDuration = 0;
        var m_top = 0;
        var m_smallWidth = 0;
        var m_largeWidth = 0;
        var m_height = 0;

        var m_leftHref = null;
        var m_onLeftClick = null;
        var m_leftSmallUri = String.Empty;
        var m_isLeftSmallImage = false;
        var m_leftLargeUri = String.Empty;
        var m_isLeftLargeImage = false;

        var m_rightHref = null;
        var m_onRightClick = null;
        var m_rightSmallUri = String.Empty;
        var m_isRightSmallImage = false;
        var m_rightLargeUri = String.Empty;
        var m_isRightLargeImage = false;

        var m_leftVerse = null;
        var m_leftSmall = null;
        var m_leftLarge = null;
        var m_rightVerse = null;
        var m_rightSmall = null;
        var m_rightLarge = null;

        var m_slidingTimer = null;
        var m_slidingDelay = 1000;
        var m_slidingAnimator = null;
        var m_slidingTop = 0;
        var m_slidingSpeed = 0;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            //get parameters
            m_isFixed = !!parameters.IsFixed;
            m_slidingDuration = Math.max(10, !global.isNaN(parameters.SlidingDuration) ? Math.floor(parameters.SlidingDuration - 0) : 0);
            m_top = !global.isNaN(parameters.Top) ? parameters.Top - 0 : 0;
            m_smallWidth = !global.isNaN(parameters.SmallWidth) ? Math.floor(parameters.SmallWidth - 0) : 0;
            m_largeWidth = !global.isNaN(parameters.LargeWidth) ? Math.floor(parameters.LargeWidth - 0) : 0;
            m_height = !global.isNaN(parameters.Height) ? parameters.Height - 0 : 0;

            !DataTypeIdentity.IsUndefinedOrNull(parameters.LeftHref) && (m_leftHref = parameters.LeftHref + String.Empty);
            m_onLeftClick = DataTypeIdentity.IsFunction(parameters.OnLeftClick) ? parameters.OnLeftClick : function () { };
            !DataTypeIdentity.IsUndefinedOrNull(parameters.LeftSmallUri) && (m_leftSmallUri = parameters.LeftSmallUri + String.Empty);
            m_isLeftSmallImage = !!parameters.IsLeftSmallImage;
            !DataTypeIdentity.IsUndefinedOrNull(parameters.LeftLargeUri) && (m_leftLargeUri = parameters.LeftLargeUri + String.Empty);
            m_isLeftLargeImage = !!parameters.IsLeftLargeImage;

            !DataTypeIdentity.IsUndefinedOrNull(parameters.RightHref) && (m_rightHref = parameters.RightHref + String.Empty);
            m_onRightClick = DataTypeIdentity.IsFunction(parameters.OnRightClick) ? parameters.OnRightClick : function () { };
            !DataTypeIdentity.IsUndefinedOrNull(parameters.RightSmallUri) && (m_rightSmallUri = parameters.RightSmallUri + String.Empty);
            m_isRightSmallImage = !!parameters.IsRightSmallImage;
            !DataTypeIdentity.IsUndefinedOrNull(parameters.RightLargeUri) && (m_rightLargeUri = parameters.RightLargeUri + String.Empty);
            m_isRightLargeImage = !!parameters.IsRightLargeImage;

            //normalize small width and large width
            m_smallWidth = m_smallWidth || m_largeWidth;
            m_largeWidth = m_largeWidth || m_smallWidth;
            m_smallWidth = Math.min(m_smallWidth, m_largeWidth);
            m_largeWidth = Math.max(m_smallWidth, m_largeWidth);

            //create elements
            var visible = BrowserGeometry.GetViewportWidth() - BrowserGeometry.GetBodyWidth() - 2 * m_largeWidth > 0;
            m_leftSmall = (m_smallWidth && m_height && m_leftSmallUri) ? CreateMedia(true, m_leftLargeUri ? !visible : true, m_smallWidth, m_height, m_leftSmallUri, m_isLeftSmallImage, m_leftHref, m_onLeftClick) : null;
            m_leftLarge = (m_largeWidth && m_height && m_leftLargeUri) ? CreateMedia(true, m_leftSmallUri ? visible : true, m_largeWidth, m_height, m_leftLargeUri, m_isLeftLargeImage, m_leftHref, m_onLeftClick) : null;
            m_rightSmall = (m_smallWidth && m_height && m_rightSmallUri) ? CreateMedia(false, m_rightLargeUri ? !visible : true, m_smallWidth, m_height, m_rightSmallUri, m_isRightSmallImage, m_rightHref, m_onRightClick) : null;
            m_rightLarge = (m_largeWidth && m_height && m_rightLargeUri) ? CreateMedia(false, m_rightSmallUri ? visible : true, m_largeWidth, m_height, m_rightLargeUri, m_isRightLargeImage, m_rightHref, m_onRightClick) : null;

            //register event handlers
            RegisterMouseEvent(true, m_leftSmall, m_leftLarge, m_rightSmall, m_rightLarge);
            RegisterMouseEvent(true, m_rightSmall, m_rightLarge, m_leftSmall, m_leftLarge);
            RegisterMouseEvent(false, m_leftLarge, m_leftSmall, m_rightLarge, m_rightSmall);
            RegisterMouseEvent(false, m_rightLarge, m_rightSmall, m_leftLarge, m_leftSmall);

            if (m_largeWidth && m_height) {
                //create left and right verse
                m_leftVerse = CreateVerse(true, m_top, m_largeWidth, m_height, m_leftSmall, m_leftLarge);
                m_rightVerse = CreateVerse(false, m_top, m_largeWidth, m_height, m_rightSmall, m_rightLarge);

                //create sliding animator
                CreateSlidingAnimator();

                //register window scroll event handler
                if (m_isFixed) {
                    EventUtility.Register(global, "scroll", function () {
                        //stop sliding timer
                        if (m_slidingTimer) {
                            global.clearTimeout(m_slidingTimer);
                            m_slidingTimer = null;
                        }
                        //stop sliding animator
                        if (m_slidingAnimator.GetIsRunning()) {
                            m_slidingAnimator.Stop();
                        }
                        //create a new sliding timer
                        m_slidingTimer = global.setTimeout(function () {
                            global.clearTimeout(m_slidingTimer);
                            m_slidingTimer = null;

                            m_slidingAnimator.Start();
                        }, m_slidingDelay);
                    }, false);
                }

                //show couplet
                global.document.body.appendChild(m_leftVerse);
                global.document.body.appendChild(m_rightVerse);
            }
        }

        /*
        * Private Method Description:
        * Creates a media element of this couplet.
        * 
        * Parameters:
        * isLeft: Whether this is the left media.
        * visible: Determine whether show this media.
        * width: The width of this media.
        * height: The height of this media.
        * uri: The Uri of the media.
        *
        * Returns:
        * Return the HTML element of this media.
        */
        function CreateMedia(isLeft, visible, width, height, uri, isImage, href, onClick) {
            var media = null;
            if (isImage) {
                media = DocumentUtility.CreateElement("img", {
                    src: uri
                }, {
                    width: "100%",
                    height: "100%",
                    margin: "0px",
                    padding: "0px",
                    borderStyle: "none",
                    cursor: "pointer"
                });
                if (href) {
                    media = DocumentUtility.CreateElement("a", {
                        href: href,
                        target: "_blank"
                    }, {
                        display: "block",
                        width: "100%",
                        height: "100%",
                        margin: "0px",
                        padding: "0px"
                    }, media);
                }
                EventUtility.Register(media, "click", onClick, false);
            } else {
                media = new FlashAnchor({
                    Uri: uri,
                    Href: href,
                    Width: "100%",
                    Height: "100%",
                    Properties: {
                        quality: "high",
                        wmode: "opaque"
                    },
                    OnClick: onClick
                }).GetElement();
            }

            var div = DocumentUtility.CreateElement("div", null, {
                width: width + "px",
                height: height + "px",
                margin: "0px",
                padding: "0px",
                position: "absolute",
                top: "0px",
                visibility: visible ? "visible" : "hidden"
            }, media);
            if (isLeft) {
                div.style.left = "0px";
            } else {
                div.style.right = "0px";
            }

            return div;
        }

        /*
        * Private Method Description:
        * Register mouse event for a media.
        */
        function RegisterMouseEvent(isMouseOver, media, siblingMedia, anotherMedia, anotherSiblingMedia) {
            if (media && siblingMedia) {
                EventUtility.Register(media, isMouseOver ? "mouseover" : "mouseout", function () {
                    media.style.visibility = "hidden";
                    siblingMedia.style.visibility = "visible";

                    if (anotherMedia && anotherSiblingMedia) {
                        anotherMedia.style.visibility = "hidden";
                        anotherSiblingMedia.style.visibility = "visible";
                    }
                }, false);
            }
        }

        /*
        * Private Method Description:
        * Creates a verse of this couplet.
        * 
        * Parameters:
        * isLeft: Whether this is the left verse.
        * isFixed: Whether fix position of this verse.
        * top: The top position of this verse.
        * width: The width of this verse.
        * height: The height of this verse.
        * small: The small element when this verse has collapsed.
        * large: The large element when this verse has expanded.
        *
        * Returns:
        * Return the HTML element of this verse.
        */
        function CreateVerse(isLeft, top, width, height, small, large) {
            var div = DocumentUtility.CreateElement("div", null, {
                width: width + "px",
                height: height + "px",
                margin: "0px",
                padding: "0px",
                position: "absolute",
                top: top + "px",
                backgroundColor: "transparent",
                overflow: "hidden"
            }, [small, large]);
            if (isLeft) {
                div.style.left = "0px";
            } else {
                div.style.right = "0px";
            }

            return div;
        }

        /*
        * Private Method Description:
        * Creates the sliding animator.
        */
        function CreateSlidingAnimator() {
            m_slidingAnimator = new Animator({
                Fps: 10,
                Duration: m_slidingDuration,
                OnInitialize: function () {
                    if (m_leftVerse.offsetTop + m_height < BrowserGeometry.GetVerticalScroll()) {
                        //verses are on top of viewport
                        m_rightVerse.style.top = m_leftVerse.style.top = (m_slidingTop = BrowserGeometry.GetVerticalScroll() - m_height) + "px";
                        m_slidingSpeed = (m_height + m_top) / m_slidingDuration;
                    } else if (m_leftVerse.offsetTop > BrowserGeometry.GetViewportHeight() + BrowserGeometry.GetVerticalScroll()) {
                        //verses are on under of viewport
                        m_rightVerse.style.top = m_leftVerse.style.top = (m_slidingTop = BrowserGeometry.GetVerticalScroll() + BrowserGeometry.GetViewportHeight()) + "px";
                        m_slidingSpeed = (m_top - BrowserGeometry.GetViewportHeight()) / m_slidingDuration;
                    } else {
                        //verses are within viewport
                        m_slidingTop = m_leftVerse.offsetTop;
                        m_slidingSpeed = (BrowserGeometry.GetVerticalScroll() + m_top - m_leftVerse.offsetTop) / m_slidingDuration;
                    }
                },
                OnFrame: function (elapsedTime) {
                    m_leftVerse.style.top = m_rightVerse.style.top = (m_slidingTop + Math.floor(m_slidingSpeed * elapsedTime)) + "px";
                },
                OnDispose: function () {
                    m_rightVerse.style.top = m_leftVerse.style.top = (BrowserGeometry.GetVerticalScroll() + m_top) + "px";
                }
            });
        }
    };

    /***************** SimpleFixedSlidesPlayer Class Definition *****************/

    /*
    * Class Description:
    * Represents a simple slides player which has no animation when switch slides.
    *
    * Constructor Parameters:
    * width: The width of this slides player;
    * height: The height of this slides player.
    * interval：The miliseconds between current picture and next picture.
    * slides: The array of slides which is a object array as follow:
    * {
    *   Title: The title of this slide.
    *   ImageUri: The image URI of this slide.
    *   TargetUri: The target URI of this slide.
    * }
    */
    var SimpleFixedSlidesPlayer = Xphter.SimpleFixedSlidesPlayer = function (width, height, interval, slides) {
        var m_width = 0;
        var m_height = 0;
        var m_interval = 0;
        var m_slides = [];

        var m_selectors = [];
        var m_playerElement = null;
        var m_anchor = null;
        var m_image = null;

        var m_timer = null;
        var m_currentIndex = -1;
        var m_isPlaying = false;

        //Event: CurrentSlideChanged
        var m_currentSlideChanged = new Event(this, "CurrentSlideChanged");

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            //get parameters
            m_width = !global.isNaN(width) ? Math.floor(width - 0) : 0;
            m_height = !global.isNaN(height) ? Math.floor(height - 0) : 0;
            m_interval = Math.max(0, !global.isNaN(interval) ? Math.floor(interval - 0) : 0);

            if (DataTypeIdentity.IsArray(slides)) {
                for (var i = 0; i < slides.length; i++) {
                    slides[i] && m_slides.push(slides[i]);
                }
            } else if (DataTypeIdentity.IsNotNullObject(slides)) {
                m_slides.push(slides);
            }

            //create slides selectors
            var selector = null;
            if (m_slides.length > 1) {
                for (var i = 0; i < m_slides.length; i++) {
                    selector = DocumentUtility.CreateElement("span", null, {
                        display: "inline-block",
                        width: "20px",
                        height: "20px",
                        borderLeft: "2px solid #FFFFFF",
                        cursor: "pointer",
                        lineHeight: "20px",
                        textAlign: "center",
                        backgroundColor: "#FFFFFF",
                        opacity: "0.5",
                        filter: "alpha(opacity=50)"
                    }, (i + 1).toString());
                    selector._selectorIndex = i;
                    EventUtility.Register(selector, "mouseover", function () {
                        StopPlay();
                        Switch(this._selectorIndex);
                    }, false);
                    EventUtility.Register(selector, "mouseout", function () {
                        PlaySlides();
                    }, false);
                    m_selectors.push(selector);
                }
            }

            //create selectors container
            var selectorContainer = m_slides.length > 1 ? DocumentUtility.CreateElement("div", null, {
                width: "auto",
                height: "auto",
                margin: "0px",
                padding: "0px",
                position: "absolute",
                right: "0px",
                bottom: "0px"
            }, m_selectors) : null;

            //create player elements
            m_image = m_slides.length ? DocumentUtility.CreateElement("img", null, {
                width: m_width + "px",
                height: m_height + "px",
                margin: "0px",
                padding: "0px",
                borderStyle: "none"
            }) : null;
            m_anchor = m_slides.length ? DocumentUtility.CreateElement("a", {
                target: "_blank"
            }, null, m_image) : null;
            m_playerElement = DocumentUtility.CreateElement("div", null, {
                width: m_width + "px",
                height: m_height + "px",
                margin: "0px",
                padding: "0px",
                position: "relative",
                left: "0px",
                top: "0px"
            }, m_slides.length ? [m_anchor, selectorContainer] : null);
        }

        /*
        * Private Method Description:
        * Switch to the specified slide.
        *
        * Parameters:
        * index: The index of the slide which will be visible.
        */
        function Switch(index) {
            var slide = m_slides[index];
            m_image.src = slide.ImageURI;
            m_image.alt = slide.Title;
            m_image.title = slide.Title;
            m_anchor.href = slide.TargetURI;

            if (m_currentIndex >= 0) {
                var previousSelector = m_selectors[m_currentIndex];
                previousSelector.style.backgroundColor = "#FFFFFF";
                previousSelector.style.opacity = "0.5";
                previousSelector.style.filter = "alpha(opacity=50)";
            }

            if (m_selectors.length) {
                var currentSelector = m_selectors[index];
                currentSelector.style.backgroundColor = "#FF0000";
                currentSelector.style.opacity = "1";
                currentSelector.style.filter = "alpha(opacity=100)";
            }

            m_currentIndex = index;
            m_currentSlideChanged.Raise();
        }

        /*
        * Private Method Description:
        * Start to play these slides.
        */
        function PlaySlides() {
            if (m_isPlaying || !m_slides.length) {
                return;
            }

            m_isPlaying = true;
            if (m_currentIndex < 0) {
                Switch(0);
            }
            if (m_slides.length > 1) {
                m_timer = global.setInterval(function () {
                    Switch((m_currentIndex + 1) % m_slides.length);
                }, m_interval);
            }
        }

        /*
        * Private Method Description:
        * Stop to play these slides.
        */
        function StopPlay() {
            if (!m_isPlaying) {
                return;
            }

            m_isPlaying = false;
            if (m_timer) {
                global.clearInterval(m_timer);
                m_timer = null;
            }
        }

        /*
        * Method Description:
        * Gets whether this slides player is playing.
        */
        this.GetIsPlaying = function () {
            return m_isPlaying;
        };

        /*
        * Method Description:
        * Gets current slide.
        */
        this.GetCurrentSlide = function () {
            return m_currentIndex >= 0 ? m_slides[m_currentIndex] : null;
        };

        /*
        * Method Description:
        * Gets the HTML element of this slides player.
        */
        this.GetPlayerElement = function () {
            return m_playerElement;
        };

        /*
        * Property Description:
        * Gets CurrentSlideChanged event.
        */
        this.GetCurrentSlideChangedEvent = function () {
            return m_currentSlideChanged;
        };

        /*
        * Method Description:
        * Start to play these slides.
        */
        this.Play = PlaySlides;

        /*
        * Method Description:
        * Stop to play these slides.
        */
        this.Stop = StopPlay;

        /*
        * Method Description:
        * Replaces a existing element with this player control.
        * 
        * Parameters:
        * element: The element will be replaced.
        */
        this.Replace = function (element) {
            element && element.parentNode && element.parentNode.replaceChild(m_playerElement, element);
        };

        /*
        * Method Description:
        * Places the player control in the specified control.
        * 
        * Parameters:
        * element: The element will contain this player control.
        */
        this.Place = function (element) {
            element && element.appendChild && element.appendChild(m_playerElement);
        };
    };

    /***************** TabControl Class Definition *****************/

    /*
    * Class Description:
    * Represents a TabControl.
    *
    * Constructor Parameters:
    * parameters: The parameters for constructor which is a object as follow:
    * {
    *   TabPages: The array of tab and pages.    
    *   IsSelectOnMouseOver: Determines whether select a tab when mouse enter it.
    *   IsHideUnselectedPages: Determines whether hide unselected pages.
    *   IsAutoSwitch: Determines whether automatic select next page after a specified time.
    *   AutoSwitchInterval: The automatic switching interval in millisecond.
    *   TabContainer: The tabs container, this argument muse be appear if you want to change the TabControl.
    *   PageContainer: The pages container, this argument muse be appear if you want to change the TabControl.
    * }
    *
    * TabPages: The array of tab pages which is a object array as follow:
    * {
    *   Tab: The tab element.
    *   Page: The page element.
    *   IsSelected: Determines whether this tab is selected.
    * }
    */
    var TabControl = Xphter.TabControl = function (parameters) {
        var m_tabPages = [];
        var m_isSelectOnMouseOver = false;
        var m_isHideUnselectedPages = true;

        var m_isAutoSwitch = false;
        var m_autoSwitchInterval = 1000;
        var m_isPageMouseOver = false;
        var m_autoSwitchTimer = null;

        var m_tabContainer = null;
        var m_pageContainer = null;
        var m_selectedIndex = -1;

        //Event: SelectedTabChanged
        var m_selectedTabChanged = new Event(this, "SelectedTabChanged");

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            parameters.TabPages && parameters.TabPages.ForEach(function (item, index) {
                if (!item) {
                    return;
                }

                item.Tab._tabIndex = m_tabPages.length;
                m_tabPages.push(item);
            });
            m_isSelectOnMouseOver = !!parameters.IsSelectOnMouseOver;
            !DataTypeIdentity.IsUndefinedOrNull(parameters.IsHideUnselectedPages) && (m_isHideUnselectedPages = !!parameters.IsHideUnselectedPages);
            m_isAutoSwitch = !!parameters.IsAutoSwitch;
            !global.isNaN(parameters.AutoSwitchInterval) && (m_autoSwitchInterval = Math.max(m_autoSwitchInterval, parameters.AutoSwitchInterval - 0));
            m_tabContainer = parameters.TabContainer;
            m_pageContainer = parameters.PageContainer;

            //routine used to control tabs            
            var eventType = m_isSelectOnMouseOver ? "mouseover" : "click";
            m_tabPages.ForEach(function (item, index) {
                EventUtility.Register(item.Tab, eventType, function (event) {
                    Select(event.currentTarget._tabIndex);
                }, false);
                if (m_isAutoSwitch) {
                    EventUtility.Register(item.Page, "mouseover", function (event) {
                        m_isPageMouseOver = true;
                        if (m_autoSwitchTimer) {
                            global.clearTimeout(m_autoSwitchTimer);
                            m_autoSwitchTimer = null;
                        }
                    }, false);
                    EventUtility.Register(item.Page, "mouseout", function (event) {
                        if (!DocumentUtility.IsChild(event.relatedTarget, event.currentTarget)) {
                            m_isPageMouseOver = false;
                            m_autoSwitchTimer = global.setTimeout(SelectNextTab, m_autoSwitchInterval);
                        }
                    }, false);
                }
            });
        }

        /*
        * Private Method Description:
        * Select the specified tab page.
        * 
        * Parameters:
        * index: The index of tab page which will be selected.
        */
        function Select(index) {
            if (index > m_tabPages.length) {
                throw new Error("index is greater than total tab number.");
            }
            if ((index = Math.max(-1, index)) == m_selectedIndex) {
                return;
            }

            m_tabPages.ForEach(function (item) {
                m_isHideUnselectedPages && (item.Page.style.display = "none");
                item.IsSelected = false;
            });
            if (index >= 0) {
                m_isHideUnselectedPages && (m_tabPages[index].Page.style.display = String.Empty);
                m_tabPages[index].IsSelected = true;
            }

            var unselectedIndex = m_selectedIndex;
            m_selectedIndex = index;
            m_selectedTabChanged.Raise({
                SelectedIndex: index,
                UnselectedIndex: unselectedIndex,
                SelectedTab: index >= 0 ? m_tabPages[index].Tab : null,
                UnselectedTab: unselectedIndex >= 0 && unselectedIndex < m_tabPages.length ? m_tabPages[unselectedIndex].Tab : null
            });

            if (m_isAutoSwitch) {
                m_autoSwitchTimer && global.clearTimeout(m_autoSwitchTimer);
                !m_isPageMouseOver && (m_autoSwitchTimer = global.setTimeout(SelectNextTab, m_autoSwitchInterval));
            }
        }

        /*
        * Method Description:
        * Selected the default tab.
        */
        function SelectDefaultTab() {
            var selectedIndex = -1;
            for (var i = 0; i < m_tabPages.length; i++) {
                if (m_tabPages[i].IsSelected) {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex >= 0) {
                Select(selectedIndex);
            } else if (m_tabPages.length > 0) {
                Select(0);
            }
        }

        /*
        * Method Description:
        * Selected next tab.
        */
        function SelectNextTab() {
            if (m_selectedIndex >= 0) {
                if (m_selectedIndex == m_tabPages.length - 1) {
                    Select(0);
                } else {
                    Select(m_selectedIndex + 1);
                }
            } else if (m_tabPages.length > 0) {
                Select(0);
            }
        }

        /*
        * Method Description:
        * Selected previous tab.
        */
        function SelectPreviousTab() {
            if (m_selectedIndex >= 0) {
                if (m_selectedIndex == 0) {
                    Select(m_tabPages.length - 1);
                } else {
                    Select(m_selectedIndex - 1);
                }
            } else if (m_tabPages.length > 0) {
                Select(0);
            }
        }

        /*
        * Private Method Description:
        * Validate whether can modify this TabControl.
        */
        function ValidateModify() {
            if (!m_tabContainer) {
                throw new Error("tab container is not specified.");
            }
            if (!m_pageContainer) {
                throw new Error("page container is not specified.");
            }
        }

        /*
        * Method Description:
        * Selected the default tab.
        */
        this.SelectDefault = SelectDefaultTab;

        /*
        * Method Description:
        * Selected next tab.
        */
        this.SelectNext = SelectNextTab;

        /*
        * Method Description:
        * Selected previous tab.
        */
        this.SelectPrevious = SelectPreviousTab;

        /*
        * Method Description:
        * Appends the specified tab page.
        * 
        * Parameters:
        * tabPage: The tab page object to append which is a object as follow:
        * {
        *   Tab: The tab element.
        *   Page: The page element.
        *   IsSelected: Determines whether this tab is selected.
        * }
        */
        this.Append = function (tabPage) {
            this.Insert(tabPage, m_tabPages.length);
        };

        /*
        * Method Description:
        * Inserts the specified tab page to the specified position.
        * 
        * Parameters:
        * tabPage: The tab page object to insert.
        * index: The position to insert.
        */
        this.Insert = function (tabPage, index) {
            if (!tabPage) {
                throw new Error("tabPage is undefined.");
            }
            if (index < 0 || index > m_tabPages.length) {
                throw new Error("index is less than zero or greater than total tab number.");
            }
            ValidateModify();

            m_tabPages.splice(index, 0, tabPage);
            for (var i = index; i < m_tabPages.length; i++) {
                m_tabPages[i].Tab._tabIndex = i;
            }

            tabPage.Page.style.display = "none";
            m_tabContainer.insertBefore(tabPage.Tab, index + 1 < m_tabPages.length ? m_tabPages[index + 1].Tab : null);
            m_pageContainer.appendChild(tabPage.Page);
            EventUtility.Register(tabPage.Tab, m_isSelectOnMouseOver ? "mouseover" : "click", function (event) {
                Select(event.currentTarget._tabIndex);
            }, false);

            m_selectedIndex >= index && ++m_selectedIndex;
            tabPage.IsSelected && Select(index);
        };

        /*
        * Method Description:
        * Removes the specified tab.
        * 
        * Parameters:
        * index: The position to remove.
        */
        this.RemoveTab = function (tab) {
            if (!tab) {
                throw new Error("tab is undefined.");
            }

            var tabPage = m_tabPages.Where(function (item, index) {
                return item.Tab == tab;
            }).FirstOrNull();
            if (!tabPage) {
                throw new Error("tab is not in this TabControl.");
            }

            this.RemoveAt(tabPage.Tab._tabIndex);
        };

        /*
        * Method Description:
        * Removes the tab page at the specified position.
        * 
        * Parameters:
        * index: The position to remove.
        */
        this.RemoveAt = function (index) {
            if (index < 0 || index >= m_tabPages.length) {
                throw new Error("index is less than zero or not less than total tab number.");
            }
            ValidateModify();

            var tabPage = m_tabPages.splice(index, 1)[0];
            for (var i = index; i < m_tabPages.length; i++) {
                m_tabPages[i].Tab._tabIndex = i;
            }
            m_tabContainer.removeChild(tabPage.Tab);
            m_pageContainer.removeChild(tabPage.Page);

            if (m_tabPages.length > 0) {
                if (m_selectedIndex > index) {
                    --m_selectedIndex;
                } else if (m_selectedIndex == index) {
                    m_selectedIndex = -1;
                    if (index >= m_tabPages.length) {
                        Select(index - 1);
                    } else {
                        Select(index);
                    }
                }
            } else {
                Select(-1);
            }
        };

        /*
        * Method Description:
        * Removes all tab pages.
        */
        this.Clear = function () {
            m_tabPages.ForEach(function (item, index) {
                var tab = item.Tab;
                var page = item.Page;
                tab.parentNode.removeChild(tab);
                page.parentNode.removeChild(page);
            });

            m_tabPages.length = 0;
            Select(-1);
        };

        /*
        * Method Description:
        * Gets number of tabs.
        */
        this.GetTabsCount = function () {
            return m_tabPages.length;
        };

        /*
        * Method Description:
        * Gets all tabs.
        */
        this.GetTabs = function () {
            return m_tabPages.Select(function (item) {
                return item.Tab;
            });
        };

        /*
        * Method Description:
        * Gets all tabs.
        */
        this.GetPages = function () {
            return m_tabPages.Select(function (item) {
                return item.Page;
            });
        };

        /*
        * Method Description:
        * Gets selected index.
        */
        this.GetSelectedIndex = function () {
            return m_selectedIndex;
        };

        /*
        * Method Description:
        * Sets selected index.
        */
        this.SetSelectedIndex = function (index) {
            Select(index - 0);
        };

        /*
        * Method Description:
        * Gets a value to indicate whether switch tabls automaticly.
        */
        this.GetIsAutoSwitch = function () {
            return m_isAutoSwitch;
        };

        /*
        * Method Description:
        * Sets a value to indicate whether switch tabls automaticly.
        */
        this.SetIsAutoSwitch = function (value) {
            if (!(m_isAutoSwitch = !!value)) {
                if (m_autoSwitchTimer) {
                    global.clearTimeout(m_autoSwitchTimer);
                    m_autoSwitchTimer = null;
                }
            }
        };

        /*
        * Method Description:
        * Gets SelectedTabChanged event.
        */
        this.GetSelectedTabChangedEvent = function () {
            return m_selectedTabChanged;
        };
    };

    /***************** Window Class Definition *****************/

    /*
    * Class Description:
    * Represents a window.
    *
    * Constructor Parameters:
    *
    * parameters: The parameters for constructor which is a object as follow:
    * {
    *   OnCreateControlBar: The callback function when creating the control bar.
    *   OnSetTitle: The callback function when setting window title.
    *   OnCreateLeftBorder: The callback function when creating the left border.
    *   OnCreateWindowContent: The callback function when creating the window content.
    *   OnCreateRightBorder: The callback function when creating the right border.
    *   OnCreateBottomBorder: The callback function when creating the bottom border.
    *   IsShowInTopWindow: Determines whether this window will be displayed in top-level window.
    * }
    */
    var Window = Xphter.Window = function (parameters) {
        var m_global = null;

        var m_windowElement = null;
        var m_controlBar = null;
        var m_leftBorder = null;
        var m_contentElement = null;
        var m_rightBorder = null;
        var m_bottomBorder = null;
        var m_rightReference = null;
        var m_bottomReference = null;

        var m_onCreateControlBar = null;
        var m_onSetTitle = null;
        var m_onCreateLeftBorder = null;
        var m_onCreateWindowContent = null;
        var m_onCreateRightBorder = null;
        var m_onCreateBottomBorder = null;
        var m_isShowInTopWindow = false;

        var m_controlBarHeight = 35;
        var m_closeButtonSize = 24;
        var m_borderWidth = 12;
        var m_titleColor = "#000000";
        var m_borderColor = "#E6E8FA";

        var m_isMouseDown = false;
        var m_mouseX = 0;
        var m_mouseY = 0;

        var m_dialogShade = null;
        var m_overflow = null;

        //Event: Shown
        var m_shown = new Event(this, "Shown");
        //Event Closed
        var m_closed = new Event(this, "Closed");

        var m_isShown = false;
        var m_returnValue = null;
        var m_onShown = null;
        var m_onClosed = null;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            //analyze parameters
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Window parameters is undefined");
            }
            DataTypeIdentity.IsFunction(parameters.OnCreateControlBar) && (m_onCreateControlBar = parameters.OnCreateControlBar);
            DataTypeIdentity.IsFunction(parameters.OnSetTitle) && (m_onSetTitle = parameters.OnSetTitle);
            DataTypeIdentity.IsFunction(parameters.OnCreateLeftBorder) && (m_onCreateLeftBorder = parameters.OnCreateLeftBorder);
            DataTypeIdentity.IsFunction(parameters.OnCreateWindowContent) && (m_onCreateWindowContent = parameters.OnCreateWindowContent);
            DataTypeIdentity.IsFunction(parameters.OnCreateRightBorder) && (m_onCreateRightBorder = parameters.OnCreateRightBorder);
            DataTypeIdentity.IsFunction(parameters.OnCreateBottomBorder) && (m_onCreateBottomBorder = parameters.OnCreateBottomBorder);
            if (m_isShowInTopWindow = !!parameters.IsShowInTopWindow) {
                if (!DataTypeIdentity.IsNotNullObject((m_global = global.top).Xphter)) {
                    throw new Error("Top window not references Xphter.js");
                }
            } else {
                m_global = global;
            }

            //create window element
            m_windowElement = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                position: "fixed",
                _position: "absolute",
                overflow: "hidden",
                backgroundColor: "#FFFFFF"
            });

            //avoid a bug in IE6: SELECT element covers all DIV elements.
            if (m_global.Xphter.BrowserCapability.IsIE && m_global.Xphter.BrowserCapability.MajorVersion == 6) {
                m_windowElement.appendChild(m_global.Xphter.DocumentUtility.CreateElement("iframe", {
                    width: "100%",
                    height: "100%",
                    frameborder: "0",
                    marginwidth: "0px",
                    marginheight: "0px"
                }, {
                    borderStyle: "none",
                    margin: "0px",
                    padding: "0px",
                    position: "absolute",
                    left: "0px",
                    top: "0px",
                    zIndex: "-1"
                }));
            }

            //create default control bar
            m_controlBar = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                height: m_controlBarHeight + "px",
                margin: "0px",
                padding: "0px",
                border: "2px solid #777777",
                borderBottomStyle: "none",
                overflow: "hidden",
                backgroundColor: m_borderColor,
                cursor: "move"
            });

            //create window title
            var windowTitle = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                width: "50%",
                height: "100%",
                lineHeight: m_controlBarHeight + "px",
                margin: "0px 0px 0px " + Math.floor((m_global.Xphter.BrowserCapability.IsIE && m_global.Xphter.BrowserCapability.MajorVersion == 6 ? 0.5 : 1) * m_borderWidth) + "px",
                padding: "0px",
                cssFloat: "left",
                styleFloat: "left",
                color: m_titleColor,
                overflow: "hidden"
            });
            m_controlBar.appendChild(windowTitle);

            //create close button
            var closeButton = m_global.Xphter.DocumentUtility.CreateElement("div", {
                title: "Close"
            }, {
                width: m_closeButtonSize + "px",
                height: "100%",
                margin: "0px " + Math.floor((m_global.Xphter.BrowserCapability.IsIE && m_global.Xphter.BrowserCapability.MajorVersion == 6 ? 0.5 : 1) * m_borderWidth) + "px 0px 0px",
                padding: "0px",
                cssFloat: "right",
                styleFloat: "right",
                cursor: "pointer",
                fontSize: m_closeButtonSize + "px",
                fontWeight: "bolder",
                textAlign: "center",
                lineHeight: m_controlBarHeight + "px",
                overflow: "hidden"
            }, "×");
            m_global.Xphter.EventUtility.Register(closeButton, "click", function () {
                CloseWindow();
            });
            m_global.Xphter.EventUtility.Register(closeButton, "mouseover", function () {
                this.style.color = "#FF0000";
            });
            m_global.Xphter.EventUtility.Register(closeButton, "mouseout", function () {
                this.style.color = "#000000";
            });
            m_controlBar.appendChild(closeButton);
            m_controlBar.appendChild(m_global.Xphter.DocumentUtility.CreateClearBoth());

            //invoke the callback function
            var value = m_onCreateControlBar ? m_onCreateControlBar(m_controlBar, CloseWindow) : true;
            if (!DataTypeIdentity.IsUndefined(value) && !value) {
                m_controlBar = null;
            }
            if (m_controlBar != null) {
                m_controlBar.style.margin = "0px";
                m_global.Xphter.EventUtility.Register(m_controlBar, "mousedown", onMouseDown);
                m_windowElement.appendChild(m_controlBar);

                !m_onSetTitle && (m_onSetTitle = function (title) {
                    windowTitle.title = (title || String.Empty) + String.Empty;
                    windowTitle.innerHTML = HtmlEncode(title);
                });
            }

            //create default left border
            m_leftBorder = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                width: m_borderWidth + "px",
                height: "10px",
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                borderLeft: "2px solid #777777",
                cssFloat: "left",
                styleFloat: "left",
                backgroundColor: m_borderColor,
                cursor: "move"
            });

            //invoke the callback function
            value = m_onCreateLeftBorder ? m_onCreateLeftBorder(m_leftBorder) : true;
            if (!DataTypeIdentity.IsUndefined(value) && !value) {
                m_leftBorder = null;
            }
            if (m_leftBorder != null) {
                m_leftBorder.style.marginTop = "0px";
                m_leftBorder.style.marginBottom = "0px";
                m_leftBorder.style.marginLeft = "0px";
                m_global.Xphter.EventUtility.Register(m_leftBorder, "mousedown", onMouseDown);
                m_windowElement.appendChild(m_leftBorder);
            }

            //create default right border
            m_rightBorder = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                width: m_borderWidth + "px",
                height: "10px",
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                borderRight: "2px solid #777777",
                cssFloat: "left",
                styleFloat: "left",
                backgroundColor: m_borderColor,
                cursor: "move"
            });

            //invoke the callback function
            value = m_onCreateRightBorder ? m_onCreateRightBorder(m_rightBorder) : true;
            if (!DataTypeIdentity.IsUndefined(value) && !value) {
                m_rightBorder = null;
            }
            if (m_rightBorder != null) {
                m_rightBorder.style.marginTop = "0px";
                m_rightBorder.style.marginBottom = "0px";
                m_rightBorder.style.marginRight = "0px";
                m_global.Xphter.EventUtility.Register(m_rightBorder, "mousedown", onMouseDown);
                m_windowElement.appendChild(m_rightBorder);
            }

            //create right reference element
            m_windowElement.appendChild(m_rightReference = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                cssFloat: "left",
                styleFloat: "left",
                fontSize: "0px",
                lineHeight: "0px"
            }));
            m_windowElement.appendChild(m_global.Xphter.DocumentUtility.CreateClearBoth());

            //create default bottom border      
            m_bottomBorder = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                height: m_borderWidth + "px",
                margin: "0px",
                padding: "0px",
                border: "2px solid #777777",
                borderTopStyle: "none",
                backgroundColor: m_borderColor,
                cursor: "move"
            });

            //invoke the callback function
            value = m_onCreateBottomBorder ? m_onCreateBottomBorder(m_bottomBorder) : true;
            if (!DataTypeIdentity.IsUndefined(value) && !value) {
                m_bottomBorder = null;
            }
            if (m_bottomBorder != null) {
                m_bottomBorder.style.margin = "0px";
                m_global.Xphter.EventUtility.Register(m_bottomBorder, "mousedown", onMouseDown);
                m_windowElement.appendChild(m_bottomBorder);
            }

            //create bottom reference element
            m_windowElement.appendChild(m_bottomReference = m_global.Xphter.DocumentUtility.CreateClearBoth());
        }

        /*
        * Private Method Description:
        * Mouse down event handler.
        */
        function onMouseDown(e) {
            m_isMouseDown = true;
            m_mouseX = e.clientX;
            m_mouseY = e.clientY;
        }

        /*
        * Private Method Description:
        * Mouse move event handler.
        */
        function onMouseMove(e) {
            if (!m_isMouseDown) {
                return;
            }

            var x = e.clientX;
            var y = e.clientY;
            m_windowElement.style.left = (m_global.parseInt(m_windowElement.style.left) + x - m_mouseX) + "px";
            m_windowElement.style.top = (m_global.parseInt(m_windowElement.style.top) + y - m_mouseY) + "px";
            m_mouseX = x;
            m_mouseY = y;
        }

        /*
        * Private Method Description:
        * Mouse up event handler.
        */
        function onMouseUp(e) {
            m_isMouseDown = false;
        }

        /*
        * Private Method Description:
        * Close window.
        *
        * Parameters:
        * returnValue: The return value of this window.
        */
        function CloseWindow(returnValue) {
            m_global.Xphter.EventUtility.Unregister(m_global.document.body, "mousemove", onMouseMove);
            m_global.Xphter.EventUtility.Unregister(m_global.document.body, "mouseup", onMouseUp);
            if (m_dialogShade) {
                m_global.document.body.removeChild(m_dialogShade);
                m_dialogShade = null;

                --m_global.Xphter.Window.g_zIndex;
                // resotre scroll bar
                !(--m_global.Xphter.Window.g_shadeLevel) && ((m_global.document.documentElement || m_global.document.body).style.overflow = m_overflow);
            }
            m_global.document.body.removeChild(m_windowElement);
            --m_global.Xphter.Window.g_zIndex;

            m_closed.Raise(m_returnValue = returnValue);

            if (m_onShown) {
                m_shown.RemoveListener(m_onShown);
                m_onShown = null;
            }
            if (m_onClosed) {
                m_closed.RemoveListener(m_onClosed);
                m_onClosed = null;
            }

            m_isShown = false;
        }

        /*
        * Method Description:
        * Show this window.
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onShown: A callback function when window has shown, the window element, with and height will pass to it.
        * onClosed: A callback function when window closed, the return value will pass to it.
        * width: The window with which only effect when the window content is a URI.
        * height: The window height which only effect when the window content is a URI.
        * x: The X coordinate of window position.
        * y: The Y coordinate of window position.
        */
        this.Show = function (title, content, onShown, onClosed, width, height) {
            if (!content) {
                throw new Error("Window content is undefined.");
            }

            var frameSource = null;
            if (!DataTypeIdentity.IsObject(content)) {
                if (!new Uri(frameSource = (content || String.Empty) + String.Empty).GetIsWellFormat()) {
                    throw new Error("Invalid window content URL.");
                }
                if (m_global.isNaN(width)) {
                    throw new Error("window width is undefined.");
                }
                if (m_global.isNaN(height)) {
                    throw new Error("window height is undefined.");
                }
            } else if (content.ownerDocument != m_global.document) {
                if (m_global.document.importNode) {
                    content = m_global.document.importNode(content, true);
                } else {
                    throw new Error("Browser is not support display the content from another window.");
                }
            }

            //initialize
            m_windowElement.style.zIndex = (m_global.Xphter.Window.g_zIndex++) + String.Empty;
            m_windowElement.style.left = "0px";
            m_windowElement.style.top = "0px";
            m_windowElement.style.width = String.Empty;
            m_windowElement.style.height = String.Empty;
            m_leftBorder && (m_leftBorder.style.height = "10px");
            m_contentElement && m_windowElement.removeChild(m_contentElement);
            m_rightBorder && (m_rightBorder.style.height = "10px");

            m_dialogShade && ++m_global.Xphter.Window.g_shadeLevel;
            if (m_onShown) {
                m_shown.RemoveListener(m_onShown);
                m_onShown = null;
            }
            if (m_onClosed) {
                m_closed.RemoveListener(m_onClosed);
                m_onClosed = null;
            }

            //set title
            m_onSetTitle && m_onSetTitle(title + String.Empty);

            //create content element
            if (frameSource) {
                m_contentElement = m_global.Xphter.DocumentUtility.CreateElement("iframe", {
                    src: frameSource,
                    width: Math.floor(width - 0) + "px",
                    height: Math.floor(height - 0) + "px",
                    frameBorder: "0",
                    marginHeight: "0px",
                    marginWidth: "0px"
                }, {
                    borderStyle: "none",
                    margin: "0px",
                    padding: "0px"
                });
            } else {
                m_contentElement = content;
            }

            // set content element style
            m_contentElement.style.margin = "0px";
            m_onCreateWindowContent && m_onCreateWindowContent(m_contentElement, !!frameSource);
            "none".EqualsIgnoreCase(m_global.Xphter.DocumentUtility.GetComputedStyle(m_contentElement).display) && (m_contentElement.style.display = "block");
            m_contentElement.style.cssFloat = "left";
            m_contentElement.style.styleFloat = "left";
            m_contentElement.style.visibility = "visible";
            m_windowElement.insertBefore(m_contentElement, m_rightBorder || m_rightReference);

            // construct UI element
            m_global.Xphter.EventUtility.Register(m_global.document.body, "mousemove", onMouseMove);
            m_global.Xphter.EventUtility.Register(m_global.document.body, "mouseup", onMouseUp);
            m_dialogShade && m_global.document.body.appendChild(m_dialogShade);
            m_global.document.body.appendChild(m_windowElement);

            // forbid scroll bar when show dialog
            if (m_dialogShade) {
                m_overflow = m_global.document.body.style.overflow;
                (m_global.document.documentElement || m_global.document.body).style.overflow = "hidden";
            }

            // provides CloseWindow function for frame
            frameSource && m_global.Xphter.EventUtility.Register(m_global.frames[m_global.frames.length - 1], "load", function () {
                try {
                    this["CloseWindow"] = CloseWindow;
                } catch (ex) {
                }
            }, false);

            // register event handlers
            DataTypeIdentity.IsFunction(onShown) && m_shown.AddListener(m_onShown = onShown);
            DataTypeIdentity.IsFunction(onClosed) && m_closed.AddListener(m_onClosed = onClosed);

            // set window dimession
            m_global.setTimeout(function () {
                m_isShown = true;

                // reset frame width and height
                if (frameSource) {
                    var leftCss = m_leftBorder ? m_global.Xphter.DocumentUtility.GetComputedStyle(m_leftBorder) : null;
                    var rightCss = m_rightBorder ? m_global.Xphter.DocumentUtility.GetComputedStyle(m_rightBorder) : null;
                    var contentCss = m_global.Xphter.DocumentUtility.GetComputedStyle(m_contentElement);

                    m_contentElement.width = (width - (m_leftBorder ? m_leftBorder.offsetWidth : 0) - (m_rightBorder ? m_rightBorder.offsetWidth : 0) - (leftCss ? m_global.parseFloat(leftCss.marginRight) || 0 : 0) - (rightCss ? m_global.parseFloat(rightCss.marginLeft) || 0 : 0) - (m_global.parseFloat(contentCss.marginLeft) || 0) - (m_global.parseFloat(contentCss.marginRight) || 0)) + "px";
                    m_contentElement.height = (height - (m_controlBar ? m_controlBar.offsetHeight : 0) - (m_bottomBorder ? m_bottomBorder.offsetHeight : 0) - (m_global.parseFloat(contentCss.marginTop) || 0) - (m_global.parseFloat(contentCss.marginBottom) || 0)) + "px";
                }

                // determine window size
                var w = m_rightReference.offsetLeft;
                var h = m_bottomReference.offsetTop;
                var contentWidth = m_contentElement.offsetWidth;
                var contentHeight = m_contentElement.offsetHeight;

                //window size should less than viewport size when show dialog
                if (m_dialogShade) {
                    var vw = m_global.Xphter.BrowserGeometry.GetViewportWidth();
                    if (w > vw) {
                        contentWidth += vw - w;
                        if (frameSource) {
                            m_contentElement.width = contentWidth + "px";
                        } else {
                            m_contentElement.style.width = contentWidth + "px";
                        }
                        w = vw;
                    }

                    var vh = m_global.Xphter.BrowserGeometry.GetViewportHeight();
                    if (h > vh) {
                        contentHeight += vh - h;
                        if (frameSource) {
                            m_contentElement.height = contentHeight + "px";
                        } else {
                            m_contentElement.style.height = contentHeight + "px";
                        }
                        h = vh;
                    }
                }

                //set window size
                var borderHeight = (m_bottomBorder || m_bottomReference).offsetTop - (m_controlBar ? m_controlBar.offsetTop + m_controlBar.offsetHeight : 0);
                m_leftBorder && (m_leftBorder.style.height = borderHeight + "px");
                m_rightBorder && (m_rightBorder.style.height = borderHeight + "px");
                m_windowElement.style.width = w + "px";
                m_windowElement.style.height = h + "px";

                //raise shown event
                m_global.setTimeout(function () {
                    m_shown.Raise(m_windowElement, w, h);
                }, 0);
            }, 0);
        };

        /*
        * Method Description:
        * Show a normal window.
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onClosed: A callback function when window closed, the return value will pass to it.
        * width: The window with which only effect when the window content is a URI.
        * height: The window height which only effect when the window content is a URI.
        * x: The X coordinate of window position.
        * y: The Y coordinate of window position.
        */
        this.ShowWindow = function (title, content, onClosed, width, height, x, y) {
            this.Show(title, content, function (element, w, h) {
                var left = 0, top = 0;
                var viewportWidth = m_global.Xphter.BrowserGeometry.GetViewportWidth();
                var viewportHeight = m_global.Xphter.BrowserGeometry.GetViewportHeight();

                try {
                    if (m_global.top) {
                        viewportWidth = Math.min(m_global.Xphter.BrowserGeometry.GetViewportWidth(), m_global.Xphter.BrowserGeometry.GetTargetViewportWidth(m_global.top));
                    }
                } catch (ex) {
                }
                try {
                    if (m_global.top) {
                        viewportHeight = Math.min(m_global.Xphter.BrowserGeometry.GetViewportHeight(), m_global.Xphter.BrowserGeometry.GetTargetViewportHeight(m_global.top));
                    }
                } catch (ex) {
                }
                if (m_global.isNaN(x)) {
                    left = Math.max(0, Math.floor((viewportWidth - w) / 2));
                } else {
                    left = x - 0;
                }
                if (m_global.isNaN(y)) {
                    top = Math.max(0, Math.floor((viewportHeight - h) / 4));
                } else {
                    top = y - 0;
                }
                // IE6 not supports fixed position.
                if (m_global.Xphter.BrowserCapability.IsIE && m_global.Xphter.BrowserCapability.MajorVersion == 6) {
                    left += m_global.Xphter.BrowserGeometry.GetHorizontalScroll();
                    top += m_global.Xphter.BrowserGeometry.GetVerticalScroll();
                }
                element.style.left = left + "px";
                element.style.top = top + "px";
            }, onClosed, width, height);
        };

        /*
        * Method Description:
        * Show a modal dialog window.
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onClosed: A callback function when window closed, the return value will pass to it.
        * width: The window with which only effect when the window content is a URI.
        * height: The window height which only effect when the window content is a URI.
        */
        this.ShowDialog = function (title, content, onClosed, width, height) {
            //create shade element
            m_dialogShade = m_global.Xphter.DocumentUtility.CreateElement("div", null, {
                width: "100%",
                height: "100%",
                position: "fixed",
                _position: "absolute",
                left: "0px",
                top: "0px",
                /* ignore IE6, it not supports "position: fixed" */
                //position: "absolute",
                //left: m_global.Xphter.BrowserGeometry.GetHorizontalScroll() + "px",
                //top: m_global.Xphter.BrowserGeometry.GetVerticalScroll() + "px",
                zIndex: (m_global.Xphter.Window.g_zIndex++) + String.Empty,
                backgroundColor: "#888888",
                opacity: "0.5",
                filter: "alpha(opacity=50)"
            });

            //avoid a bug in IE6: SELECT element cover all DIV elements.
            if (m_global.Xphter.BrowserCapability.IsIE && m_global.Xphter.BrowserCapability.MajorVersion == 6) {
                m_dialogShade.appendChild(m_global.Xphter.DocumentUtility.CreateElement("iframe", {
                    width: "100%",
                    height: "100%",
                    frameborder: "0",
                    marginwidth: "0px",
                    marginheight: "0px"
                }, {
                    borderStyle: "none",
                    margin: "0px",
                    padding: "0px"
                }));
            }

            this.ShowWindow(title, content, onClosed, width, height);
        };

        /*
        * Method Description:
        * Show a modal dialog window in full screen when the window content is a URI
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onClosed: A callback function when window closed, the return value will pass to it.
        */
        this.ShowFullScreenDialog = function (title, content, onClosed) {
            this.ShowDialog(title, content, onClosed, m_global.Xphter.BrowserGeometry.GetViewportWidth() - 30, m_global.Xphter.BrowserGeometry.GetViewportHeight() - 10);
        };

        /*
        * Method Description:
        * Show a modal dialog window in full X-axis when the window content is a URI
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onClosed: A callback function when window closed, the return value will pass to it.
        */
        this.ShowFullXDialog = function (title, content, onClosed, height) {
            this.ShowDialog(title, content, onClosed, m_global.Xphter.BrowserGeometry.GetViewportWidth() - 30, height);
        };

        /*
        * Method Description:
        * Show a modal dialog window in full Y-axis when the window content is a URI
        *
        * Parameters:
        * title: The window title.
        * content: The window content.
        * onClosed: A callback function when window closed, the return value will pass to it.
        */
        this.ShowFullYDialog = function (title, content, onClosed, width) {
            this.ShowDialog(title, content, onClosed, width, m_global.Xphter.BrowserGeometry.GetViewportHeight() - 10);
        };

        /*
        * Method Description:
        * Close window.
        */
        this.Close = function () {
            CloseWindow(m_returnValue);
        };

        /*
        * Method Description:
        * Gets Shown Event.
        * The window element, with and height will be passed to this event.
        */
        this.GetShownEvent = function () {
            return m_shown;
        };

        /*
        * Method Description:
        * Gets Closed Event.
        * The return value will be passed to this event.
        */
        this.GetClosedEvent = function () {
            return m_closed;
        };

        /*
        * Method Description:
        * Gets return value.
        */
        this.GetReturnValue = function () {
            return m_returnValue;
        };

        /*
        * Method Description:
        * Sets return value.
        */
        this.SetReturnValue = function (returnValue) {
            m_returnValue = returnValue;
        };

        /*
        * Method Description:
        * Gets whether this window has shown.
        */
        this.GetIsShown = function () {
            return m_isShown;
        };
    };

    Window.g_zIndex = 100;
    Window.g_shadeLevel = 0;

    /***************** ListView Class Definition *****************/

    /*
    * Class Description:
    * Represents a list.
    *
    * Constructor Parameters:
    *
    * parameters: The parameters for constructor which is a object as follow:
    * {
    *   Container: The container element of this list. All children of it will be removed.
    *   Header: The header of this list.
    *   Loading: The element used to prompt that system is loading.
    *   Footer: The footer of this list.
    *   ItemTemplate: The item template of this list.
    *   ClickableSelector: The function to get the clickable element from the item template.
    *   PropertyTemplate: The part in item template which match property template will be replaced by property value. 
    *                     Property template may like this: $property$, "property" is required.
    *   PrimaryKey: A property name, which will be used to compare each item. 
    * }
    */
    var ListView = Xphter.ListView = function (parameters) {
        var m_container = null;
        var m_display = null;
        var m_header = null;
        var m_loading = null;
        var m_footer = null;
        var m_itemTemplate = null;
        var m_clickableSelector = null;
        var m_propertyName = "property";
        var m_propertyTemplate = null;
        var m_primaryKey = null;

        var m_replaceMap = {};
        var m_items = [];
        var m_selectedIndex = -1;

        var m_insertItemsOperations = {};

        var m_itemCreated = new Event(this, "ItemCreated");
        var m_itemUpdating = new Event(this, "ItemUpdating");
        var m_checkedChanged = new Event(this, "CheckedChanged");
        var m_selectedChanged = new Event(this, "SelectedChanged");

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("ListView parameters is undefined");
            }
            if (!DataTypeIdentity.IsNotNullObject(parameters.Container)) {
                throw new Error("List container is undefined");
            }
            if (!DataTypeIdentity.IsNotNullObject(parameters.ItemTemplate)) {
                throw new Error("Template of list item is undefined.");
            }
            if (!parameters.PropertyTemplate) {
                throw new Error("Property template of list view is undefined");
            }
            if (!parameters.PrimaryKey) {
                throw new Error("Primary key of list item is undefined");
            }

            //analyze parameters
            m_container = parameters.Container;
            m_header = parameters.Header || null;
            m_loading = parameters.Loading || null;
            m_footer = parameters.Footer || null;
            m_itemTemplate = parameters.ItemTemplate;
            m_clickableSelector = DataTypeIdentity.IsFunction(parameters.ClickableSelector) ? parameters.ClickableSelector : function (node) {
                return node;
            };
            m_propertyTemplate = parameters.PropertyTemplate + String.Empty;
            m_primaryKey = parameters.PrimaryKey + String.Empty;

            //initialize list
            "none".EqualsIgnoreCase(m_container.style.display) && (m_container.style.display = String.Empty);
            m_display = m_container.style.display;
            DocumentUtility.ClearChildren(m_container);
            m_header && m_container.appendChild(m_header);
            m_footer && m_container.appendChild(m_footer);
        }

        /*
        * The innerHTML property is read-only of table and tr element in IE 6/7/8/9.
        */
        function CreateItemNodeWrapper() {
            var node = m_itemTemplate.cloneNode(true);
            node.id = String.Format("xphterListViewItem{0}{1}", new Date().getTime(), m_items.length);

            switch (node.nodeName.toLowerCase()) {
                case "th":
                case "td":
                    node = DocumentUtility.CreateElement("div", null, null, DocumentUtility.CreateElement("table", null, null, DocumentUtility.CreateElement("tbody", null, null, DocumentUtility.CreateElement("tr", null, null, node))));
                    break;
                case "tr":
                    node = DocumentUtility.CreateElement("div", null, null, DocumentUtility.CreateElement("table", null, null, DocumentUtility.CreateElement("tbody", null, null, node)));
                    break;
                case "dt":
                case "dd":
                    node = DocumentUtility.CreateElement("div", null, null, DocumentUtility.CreateElement("dl", null, null, node));
                case "li":
                    node = DocumentUtility.CreateElement("div", null, null, DocumentUtility.CreateElement("ul", null, null, node));
                    break;
                case "option":
                    node = DocumentUtility.CreateElement("div", null, null, DocumentUtility.CreateElement("select", null, null, node));
                    break;
                default:
                    node = DocumentUtility.CreateElement("div", null, null, node);
                    break;
            }

            return node;
        }

        function GetItemNodeFromWrapper(node) {
            switch (m_itemTemplate.nodeName.toLowerCase()) {
                case "th":
                case "td":
                    return node.firstChild.firstChild.firstChild.firstChild;
                case "tr":
                    return node.firstChild.firstChild.firstChild;
                case "dt":
                case "dd":
                case "li":
                case "option":
                    return node.firstChild.firstChild;
                default:
                    return node.firstChild;
            }
        }

        function OnSelectedChanged(index) {
            if (index >= m_items.length || m_selectedIndex == index) {
                return;
            }

            if ((m_selectedIndex = index) >= 0) {
                m_selectedChanged.Raise({
                    Index: index,
                    Value: m_items[index].Value,
                    Node: m_items[index].Node,
                    OtherNodes: m_items.Where(function (item, ix) {
                        return ix != index;
                    }).Select(function (item, ix) {
                        return item.Node;
                    })
                });
            } else {
                m_selectedChanged.Raise({
                    Index: index,
                    Value: null,
                    Node: null,
                    OtherNodes: m_items.Select(function (item, ix) {
                        return item.Node;
                    })
                });
            }
        }

        function OnItemRemoved(index) {
            if (index == m_selectedIndex) {
                OnSelectedChanged(-1);
                if (m_items.length > 0) {
                    OnSelectedChanged(index == m_items.length ? index - 1 : index);
                }
            } else if (index < m_selectedIndex) {
                --m_selectedIndex;
            }
        }

        function NewNode(item) {
            var value = null;
            var node = CreateItemNodeWrapper();
            var innerHTML = node.innerHTML;

            for (var name in item) {
                if (!name) {
                    continue;
                }

                if (!(name in m_replaceMap)) {
                    m_replaceMap[name] = m_propertyTemplate.ReplaceGlobal(m_propertyName, name);
                }
                innerHTML = innerHTML.ReplaceGlobal(m_replaceMap[name], DataTypeIdentity.IsUndefinedOrNull(value = item[name]) ? String.Empty : (value + String.Empty).ReplaceGlobal("\\", "\\\\"));
            }
            node.innerHTML = innerHTML;
            node = GetItemNodeFromWrapper(node);
            EventUtility.Register(m_clickableSelector(node), "click", function () {
                OnSelectedChanged(m_items.FindIndex(function (item, index) {
                    return item.Node == node;
                }));
            }, false);

            return node;
        }

        /*
        * Method Description:
        * Gets the list container element.
        */
        this.GetContainer = function () {
            return m_container;
        };

        /*
        * Method Description:
        * Gets the list header element.
        */
        this.GetHeader = function () {
            return m_header;
        };

        /*
        * Method Description:
        * Gets the list footer element.
        */
        this.GetFooter = function () {
            return m_footer;
        };

        /*
        * Method Description:
        * Prepare to refresh list: prompts user that system is loading.
        */
        this.PrepareRefresh = function () {
            DocumentUtility.ClearChildren(m_container);
            m_header && m_container.appendChild(m_header);
            m_loading && m_container.appendChild(m_loading);
            m_footer && m_container.appendChild(m_footer);
        };

        /*
        * Method Description:
        * Refresh this list with the specified data.
        *
        * Parameters:
        * values: A object array.
        */
        this.Refresh = function (values) {
            if (!DataTypeIdentity.IsArray(values)) {
                throw new Error("List values is not a array.");
            }

            DocumentUtility.ClearChildren(m_container);
            m_header && m_container.appendChild(m_header);
            m_footer && m_container.appendChild(m_footer);

            var value = null;
            var node = null;
            m_items.length = 0;
            m_selectedIndex = -2;
            for (var i = 0; i < values.length; i++) {
                if (DataTypeIdentity.IsUndefinedOrNull(value = values[i])) {
                    continue;
                }

                node = m_container.insertBefore(NewNode(value), m_footer);
                m_items.push({
                    Value: value,
                    Node: node,
                    Checked: false
                });
                m_itemCreated.Raise({
                    Index: i,
                    Value: value,
                    Node: node,
                    ClickableNode: m_clickableSelector(node)
                });
            }
            if (values.length > 0) {
                OnSelectedChanged(0);
            } else {
                OnSelectedChanged(-1);
            }
        };

        /*
        * Method Description:
        * Finish refresh list: prompts user that system has loaded.
        */
        this.FinishRefresh = function () {
            m_loading && m_loading.parentNode && m_loading.parentNode == m_container && m_container.removeChild(m_loading);
        };

        /*
        * Method Description:
        * Append the specified item to end of this list.
        *
        * Parameters:
        * value: The object will be appended.
        */
        this.Append = function (value) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }

            var index = this.IndexOf(value);
            if (index >= 0) {
                m_container.removeChild(m_items.splice(index, 1)[0].Node);
            }

            var node = m_container.insertBefore(NewNode(value), m_footer);
            m_items.push({
                Value: value,
                Node: node,
                Checked: false
            });
            m_itemCreated.Raise({
                Index: m_items.length - 1,
                Value: value,
                Node: node,
                ClickableNode: m_clickableSelector(node)
            });
            OnSelectedChanged(m_items.length - 1);
        };

        /*
        * Method Description:
        * Insert the specified item to the specified position.
        *
        * Parameters:
        * value: The object will be inserted.
        * index: The position to insert item.
        */
        this.Insert = function (value, index) {
            var _index = index - 0;

            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }
            if (_index < 0 || _index > m_items.length) {
                throw new Error("Index is out of range.");
            }

            var ix = this.IndexOf(value);
            if (ix >= 0) {
                m_container.removeChild(m_items.splice(ix, 1)[0].Node);
                if (ix < _index) {
                    --_index;
                }
            }

            var node = m_container.insertBefore(NewNode(value), _index == m_items.length ? m_footer : m_items[_index].Node);
            m_items.splice(_index, 0, {
                Value: value,
                Node: node,
                Checked: false
            });
            m_itemCreated.Raise({
                Index: _index,
                Value: value,
                Node: node,
                ClickableNode: m_clickableSelector(node)
            });
            if (_index == m_selectedIndex) {
                OnSelectedChanged(-1);
            }
            OnSelectedChanged(_index);
        };

        /*
        * Method Description:
        * Prepare to insert items: prompts user that system is loading.
        */
        this.PrepareInsertItems = function (index) {
            var _index = index - 0;

            if (_index < 0 || _index > m_items.length) {
                throw new Error("Index is out of range.");
            }
            if (_index.toString() in m_insertItemsOperations) {
                throw new Error("There is already an operation to insert items at " + _index);
            }

            var loading = null;

            if (m_loading) {
                m_container.insertBefore(loading = m_loading.cloneNode(true), _index == m_items.length ? m_footer : m_items[_index].Node);
            }

            m_insertItemsOperations[_index.toString()] = {
                Index: _index,
                Loading: loading
            };
        };

        /*
        * Method Description:
        * Insert the specified items to the specified position.
        *
        * Parameters:
        * values: The objects will be inserted.
        * index: The position to insert items.
        */
        this.InsertItems = function (values, index) {
            var _index = index - 0;

            if (!DataTypeIdentity.IsNotNullObject(values)) {
                throw new Error("List item values is null.");
            }
            if (_index < 0 || _index > m_items.length) {
                throw new Error("Index is out of range.");
            }
            if (!(_index.toString() in m_insertItemsOperations)) {
                throw new Error("There is not an operation to insert items at " + _index);
            }

            if (!values.length) {
                return;
            }

            var ix = -1;
            var value = null;
            var avaliableValues = [];

            for (var i = 0; i < values.length; i++) {
                if ((ix = this.IndexOf(value = values[i])) >= 0) {
                    this.RemoveAt(ix);

                    for (var name in m_insertItemsOperations) {
                        if (m_insertItemsOperations[name].Index > ix) {
                            m_insertItemsOperations[name].Index -= 1;
                        }
                    }
                } else {
                    avaliableValues.push(value);
                }
            }

            var operation = m_insertItemsOperations[_index.toString()];

            _index = operation.Index;
            for (var name in m_insertItemsOperations) {
                if (m_insertItemsOperations[name].Index > _index) {
                    m_insertItemsOperations[name].Index += avaliableValues.length;
                }
            }

            var node = null;
            var nextSibling = operation.Loading || (_index == m_items.length ? m_footer : m_items[_index].Node);
            for (var i = 0; i < avaliableValues.length; i++) {
                node = m_container.insertBefore(NewNode(value = avaliableValues[i]), nextSibling);
                m_items.splice(_index, 0, {
                    Value: value,
                    Node: node,
                    Checked: false
                });
                m_itemCreated.Raise({
                    Index: _index,
                    Value: value,
                    Node: node,
                    ClickableNode: m_clickableSelector(node)
                });
                ++_index;
            }

            if (_index - avaliableValues.length == m_selectedIndex) {
                OnSelectedChanged(-1);
            }
            OnSelectedChanged(_index - avaliableValues.length);
        };

        /*
        * Method Description:
        * Finish insert items: prompts user that system has loaded.
        */
        this.FinishInsertItems = function (index) {
            var _index = index - 0;

            if (_index < 0 || _index > m_items.length) {
                throw new Error("Index is out of range.");
            }
            if (!(_index.toString() in m_insertItemsOperations)) {
                throw new Error("There is not an operation to insert items at " + _index);
            }

            var operation = m_insertItemsOperations[_index.toString()];
            if (operation.Loading) {
                m_container.removeChild(operation.Loading);
            }

            delete m_insertItemsOperations[_index.toString()];
        };

        /*
        * Method Description:
        * Update this list by the specified object.
        *
        * Parameters:
        * value: A object.
        */
        this.Update = function (value) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }

            var index = this.IndexOf(value);
            if (index >= 0) {
                var node = NewNode(value);
                m_container.replaceChild(node, m_items.splice(index, 1, {
                    Value: value,
                    Node: node,
                    Checked: false
                })[0].Node);
                m_itemCreated.Raise({
                    Index: index,
                    Value: value,
                    Node: node,
                    ClickableNode: m_clickableSelector(node)
                });
                if (index == m_selectedIndex) {
                    OnSelectedChanged(-1);
                    OnSelectedChanged(index);
                }
            }
        };

        /*
        * Method Description:
        * Update the row in the specified position of the specified object.
        *
        * Parameters:
        * index: A row index.
        * value: A object.
        */
        this.UpdateAt = function (value, index) {
            var _index = index - 0;
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }

            var node = NewNode(value);
            m_container.replaceChild(node, m_items.splice(_index, 1, {
                Value: value,
                Node: node,
                Checked: false
            })[0].Node);
            m_itemCreated.Raise({
                Index: _index,
                Value: value,
                Node: node,
                ClickableNode: m_clickableSelector(node)
            });
            if (_index == m_selectedIndex) {
                OnSelectedChanged(-1);
                OnSelectedChanged(_index);
            }
        };

        /*
        * Method Description:
        * Remove row in this list which associate with the specified object.
        *
        * Parameters:
        * value: A object.
        */
        this.Remove = function (value) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }

            var index = this.IndexOf(value);
            if (index >= 0) {
                this.RemoveAt(index);
            }
        };

        /*
        * Method Description:
        * Remove row in this list by the specified primary key.
        *
        * Parameters:
        * key: A primary key.
        */
        this.RemoveByKey = function (key) {
            if (DataTypeIdentity.IsUndefined(key)) {
                throw new Error("List item key is undefined.");
            }

            var index = this.IndexOfKey(key);
            if (index >= 0) {
                this.RemoveAt(index);
            }
        };

        /*
        * Method Description:
        * Remove row in the specified position.
        *
        * Parameters:
        * index: The position to remove.
        */
        this.RemoveAt = function (index) {
            var _index = index - 0;
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }

            m_container.removeChild(m_items.splice(_index, 1)[0].Node);

            OnItemRemoved(_index);
        };

        /*
        * Method Description:
        * Sets checked state of the specified object.
        *
        * Parameters:
        * value: A object.
        */
        this.SetChecked = function (value, checked) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }

            var index = this.IndexOf(value);
            if (index >= 0) {
                this.SetCheckedAt(index, checked);
            }
        };

        /*
        * Method Description:
        * Sets checked state by the specified primary key.
        *
        * Parameters:
        * key: A primary key.
        */
        this.SetCheckedByKey = function (key, checked) {
            if (DataTypeIdentity.IsUndefined(key)) {
                throw new Error("List item key is undefined.");
            }

            var index = this.IndexOfKey(key);
            if (index >= 0) {
                this.SetCheckedAt(index, checked);
            }
        };

        /*
        * Method Description:
        * Sets checked state in the specified position.
        *
        * Parameters:
        * index: The position to set checked state.
        */
        this.SetCheckedAt = function (index, checked) {
            var _index = index - 0;
            var _checked = !!checked;
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }

            var item = m_items[index];
            if (item.Checked != _checked) {
                item.Checked = _checked;

                m_checkedChanged.Raise({
                    Index: index,
                    Value: item.Value,
                    Node: item.Node,
                    Checked: item.Checked
                });
            }
        };

        /*
        * Method Description:
        * Sets all checked states.
        */
        this.SetAllChecked = function (checked) {
            var item = null;
            var _checked = !!checked;

            for (var i = 0; i < m_items.length; i++) {
                if ((item = m_items[i]).Checked != _checked) {
                    item.Checked = _checked;

                    m_checkedChanged.Raise({
                        Index: i,
                        Value: item.Value,
                        Node: item.Node,
                        Checked: item.Checked
                    });
                }
            }
        };

        /*
        * Method Description:
        * Gets index of the specified object, return -1 if not found.
        *
        * Parameters:
        * value: A object.
        */
        this.IndexOf = function (value) {
            if (!DataTypeIdentity.IsNotNullObject(value)) {
                throw new Error("List item value is null.");
            }

            return m_items.FindIndex(function (item, index) {
                return item.Value == value || ((m_primaryKey in item.Value) && (m_primaryKey in value) && (item.Value[m_primaryKey] == value[m_primaryKey]));
            });
        };

        /*
        * Method Description:
        * Gets index of the specified primary key, return -1 if not found.
        *
        * Parameters:
        * key: a primary key.
        */
        this.IndexOfKey = function (key) {
            if (DataTypeIdentity.IsUndefined(key)) {
                throw new Error("List item key is undefined.");
            }

            return m_items.FindIndex(function (item, index) {
                return (m_primaryKey in item.Value) && (item.Value[m_primaryKey] == key);
            });
        };

        /*
        * Method Description:
        * Moves up the specified object.
        *
        * Parameters:
        * index: The position to move up.
        * rowCount: The number of rows on shift up.
        */
        this.MoveUp = function (index, rowCount) {
            var _index = index - 0;
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }
            var _rowCount = rowCount - 0;
            if (_rowCount < 0) {
                throw new Error("The number of rows on shift up is less than zero.");
            }
            if (!_rowCount || !_index) {
                return;
            }

            _rowCount = Math.min(_index, _rowCount);
            var upIndex = _index - _rowCount;

            m_container.insertBefore(m_container.removeChild(m_items[_index].Node), m_items[upIndex].Node);
            m_items.splice(upIndex, 0, m_items.splice(_index, 1)[0]);

            if (_index == m_selectedIndex || upIndex == m_selectedIndex) {
                var index = m_selectedIndex;
                OnSelectedChanged(-1);
                OnSelectedChanged(index);
            }
        };

        /*
        * Method Description:
        * Moves down the specified object.
        *
        * Parameters:
        * index: The position to move down.
        * rowCount: The number of rows on shift down.
        */
        this.MoveDown = function (index, rowCount) {
            var _index = index - 0;
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }
            var _rowCount = rowCount - 0;
            if (_rowCount < 0) {
                throw new Error("The number of rows on shift down is less than zero.");
            }
            if (!_rowCount || (_index == m_items.length - 1)) {
                return;
            }

            _rowCount = Math.min(m_items.length - 1 - _index, _rowCount);
            var downIndex = _index + _rowCount;

            m_container.insertBefore(m_container.removeChild(m_items[_index].Node), m_items[downIndex].Node.nextSibling);
            m_items.splice(downIndex, 0, m_items.splice(_index, 1)[0]);

            if (_index == m_selectedIndex || downIndex == m_selectedIndex) {
                var index = m_selectedIndex;
                OnSelectedChanged(-1);
                OnSelectedChanged(index);
            }
        };

        /*
        * Method Description:
        * Gets number of items in this ListView.
        */
        this.GetItemsCount = function () {
            return m_items.length;
        };

        /*
        * Method Description:
        * Gets value in the specified position.
        * The ItemUpdating event will be raised.
        *
        * Parameters:
        * index: The position of item.
        */
        this.GetValue = function (index) {
            var _index = index - 0;
            if (_index < 0 || _index >= m_items.length) {
                throw new Error("Index out of range.");
            }

            var item = m_items[_index];
            var args = {
                Index: index,
                Value: item.Value,
                Node: item.Node
            };
            m_itemUpdating.Raise(args);

            return item.Value = (args.Value || item.Value);
        };

        /*
        * Method Description:
        * Gets value of the specified key.
        * The ItemUpdating event will be raised.
        *
        * Parameters:
        * key: The key data.
        */
        this.GetValueByKey = function (key) {
            if (DataTypeIdentity.IsUndefined(key)) {
                throw new Error("List item key is undefined.");
            }

            var index = this.IndexOfKey(key);
            if (index < 0) {
                return null;
            }

            return this.GetValue(index);
        };

        /*
        * Method Description:
        * Gets values in the ListView.
        * The ItemUpdating event will be raised.
        */
        this.GetValues = function () {
            for (var i = 0; i < m_items.length; i++) {
                this.GetValue(i);
            }

            return m_items.Select(function (item, index) {
                return item.Value;
            });
        };

        /*
        * Method Description:
        * Shows this list.
        */
        this.Show = function () {
            m_container.style.display = m_display;
        };

        /*
        * Method Description:
        * Hides this list.
        */
        this.Hide = function () {
            m_container.style.display = "none";
        };

        /*
        * Method Description:
        * Gets the index of current selected item.
        */
        this.GetSelectedIndex = function () {
            return m_selectedIndex;
        };

        /*
        * Method Description:
        * Sets the item that is in the specified position to selected.
        */
        this.SetSelectedIndex = function (index) {
            OnSelectedChanged(Math.max(-1, index - 0));
        };

        /*
        * Method Description:
        * Gets the current selected value.
        */
        this.GetSelectedValue = function () {
            return m_selectedIndex >= 0 ? m_items[m_selectedIndex].Value : null;
        };

        /*
        * Method Description:
        * Sets the specified item to selected.
        */
        this.SetSelectedValue = function (value) {
            OnSelectedChanged(DataTypeIdentity.IsNotNullObject(value) ? this.IndexOf(value) : -1);
        };

        /*
        * Method Description:
        * Gets the current checked values.
        */
        this.GetCheckedValues = function () {
            return m_items.Where(function (item, index) {
                return item.Checked;
            }).Select(function (item, index) {
                return item.Value;
            });
        };

        /*
        * Method Description:
        * Gets the ItemCreated event which occurs after creating a item.
        * The argument object passed to event handlers as follows:
        * {Index: itemIndex, Value: itemObject, Node: htmlNode, ClickableNode: htmlNode}.
        */
        this.GetItemCreatedEvent = function () {
            return m_itemCreated;
        };

        /*
        * Method Description:
        * Gets the ItemUpdatingEvent event which occurs when retrievaling the new state of item.
        * The argument object passed to event handlers as follows:
        * {Index: itemIndex, Value: itemObject, Node: htmlNode}.
        */
        this.GetItemUpdatingEvent = function () {
            return m_itemUpdating;
        };

        /*
        * Method Description:
        * Gets the SelectedChangedEvent event which occurs when the selected item has changed.
        * The argument object passed to event handlers as follows:
        * {Index: selectedIndex, Value: selectedValue, Node: selectedNode, OtherNodes: notSelectedNodes}.
        */
        this.GetSelectedChangedEvent = function () {
            return m_selectedChanged;
        };

        /*
        * Method Description:
        * Gets the CheckedChangedEvent event which occurs when a item is checked or unchecked.
        * The argument object passed to event handlers as follows:
        * {Index: itemIndex, Value: itemValue, Node: itemNode, Checked: trueOrFalse}.
        */
        this.GetCheckedChangedEvent = function () {
            return m_checkedChanged;
        };
    };

    /***************** TreeView Class Definition *****************/

    /*
    * Class Description:
    * Represents a tree which has some nested lists.
    */
    var TreeView = Xphter.TreeView = function () {
        var m_root = null;
        var m_nodes = [];

        var m_childrenCreating = new Event(this, "ChildrenCreating");
        var m_nodeCreated = new Event(this, "NodeCreated");
        var m_nodeExpanding = new Event(this, "NodeExpanding");
        var m_nodeCollapsing = new Event(this, "NodeCollapsing");
        var m_selectedChanged = new Event(this, "SelectedChanged");

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            //initialize the abstract root node
            m_root = {
                Parent: null,
                Value: null,
                EntireNode: null,
                ContentNode: null,
                ListView: null,
                IsExpanded: true
            };
        }

        function FindNode(value) {
            if (DataTypeIdentity.IsUndefinedOrNull(value)) {
                return null;
            }

            var index = -1;
            var node = null;
            if (m_root.ListView && (index = m_root.ListView.IndexOf(value)) >= 0) {
                node = m_root;
            } else {
                for (var i = 0; i < m_nodes.length; i++) {
                    if (!(node = m_nodes[i]).ListView) {
                        continue;
                    }

                    if ((index = node.ListView.IndexOf(value)) >= 0) {
                        break;
                    }
                }
            }

            if (index >= 0) {
                var nodeValue = node.ListView.GetValue(index);
                return m_nodes.Where(function (item, ix) {
                    return item.Value == nodeValue;
                }).FirstOrNull();
            } else {
                return null;
            }
        }

        function FindSelectedNode() {
            if (!m_root.ListView) {
                return null;
            }

            return FindNode(m_root.ListView.GetSelectedValue() || m_nodes.Where(function (item, index) {
                return item.ListView && item.ListView.GetSelectedIndex() >= 0;
            }).Select(function (item, index) {
                return item.ListView.GetSelectedValue();
            }).FirstOrNull());
        }

        function InitializeChildren(node) {
            if (node.ListView) {
                return;
            }

            node.ListView = OnChildrenCreating(node);
            node.ListView.GetItemCreatedEvent().AddListener(function (args) {
                var current = FindNode(args.Value);
                if (current) {
                    current.Value = args.Value;
                    current.EntireNode = args.Node;
                    current.ContentNode = args.ClickableNode;
                    if (current.ListView) {
                        current.EntireNode.appendChild(current.ListView.GetContainer());
                    }
                } else {
                    m_nodes.push(current = {
                        Parent: node,
                        Value: args.Value,
                        EntireNode: args.Node,
                        ContentNode: args.ClickableNode,
                        ListView: null,
                        IsExpanded: false
                    });
                }

                m_nodeCreated.Raise({
                    Value: args.Value,
                    Node: args.ClickableNode
                });
            });
            node.ListView.GetSelectedChangedEvent().AddListener(function (args) {
                if (args.Value) {
                    m_nodes.Where(function (item, index) {
                        return item != node;
                    }).ForEach(function (item, index) {
                        item.ListView && item.ListView.SetSelectedIndex(-1);
                    });
                    (node != m_root) && m_root.ListView.SetSelectedIndex(-1);

                    var current = FindNode(args.Value);
                    m_selectedChanged.Raise({
                        Value: current.Value,
                        Node: current.ContentNode,
                        OtherNodes: m_nodes.Where(function (item, index) {
                            return item != current;
                        }).Select(function (item, index) {
                            return item.ContentNode;
                        })
                    });
                } else {
                    if (!FindSelectedNode()) {
                        m_selectedChanged.Raise({
                            Value: null,
                            Node: null,
                            OtherNodes: m_nodes.Select(function (item, index) {
                                return item.ContentNode;
                            })
                        });
                    }
                }
            });
            node.EntireNode && node.EntireNode.appendChild(node.ListView.GetContainer());
        }

        function RemoveByParent(node) {
            if (node == m_root) {
                m_nodes.length = 0;
                return;
            }

            var nodes = [];
            var parents = [node];
            while (parents.length > 0) {
                nodes = m_nodes.Where(function (item, index) {
                    return parents.Contains(item.Parent);
                });
                nodes.ForEach(function (item, index) {
                    m_nodes.Remove(item);
                });
                parents = nodes;
            }
        }

        function OnChildrenCreating(node) {
            var args = {
                Value: node.Value,
                ListParameters: {}
            };

            m_childrenCreating.Raise(args);
            var parameters = args.ListParameters;
            if (parameters) {
                if (node.Value && parameters.Container) {
                    parameters.Container = parameters.Container.cloneNode(false);
                }
                if (parameters.Header) {
                    parameters.Header = parameters.Header.cloneNode(true);
                }
                if (parameters.ItemTemplate) {
                    parameters.ItemTemplate = parameters.ItemTemplate.cloneNode(true);
                }
                if (parameters.Footer) {
                    parameters.Footer = parameters.Footer.cloneNode(true);
                }
                if (parameters.Loading) {
                    parameters.Loading = parameters.Loading.cloneNode(true);
                }
                if (!DataTypeIdentity.IsFunction(parameters.ClickableSelector)) {
                    parameters.ClickableSelector = function (node) {
                        return DocumentUtility.GetFirstElement(node);
                    };
                }
            }

            return new ListView(parameters);
        }

        function OnNodeExpanding(node) {
            if (!node || node.IsExpanded) {
                return;
            }

            node.IsExpanded = true;
            node.ListView && node.ListView.Show();
            m_nodeExpanding.Raise({
                Value: node.Value,
                Node: node.ContentNode,
                IsInitialized: !!node.ListView
            });
        }

        function OnNodeCollapsing(node) {
            if (!node || !node.IsExpanded) {
                return;
            }

            node.IsExpanded = false;
            node.ListView && node.ListView.Hide();
            m_nodeCollapsing.Raise({
                Value: node.Value,
                Node: node.ContentNode
            });
        }

        /*
        * Method Description:
        * Prepare to refresh list: prompts user that system is loading.
        *
        * Parameters:
        * value: The value of the node that prepares to refresh.
        */
        this.PrepareRefresh = function (value) {
            var node = value ? FindNode(value) : m_root;
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            InitializeChildren(node);
            node.ListView.PrepareRefresh();
        };

        /*
        * Method Description:
        * Refresh this tree with the specified data.
        *
        * Parameters:
        * value: The value of the node will be refreshed.
        * data: A object array.
        */
        this.Refresh = function (value, data) {
            var node = value ? FindNode(value) : m_root;
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            RemoveByParent(node);
            InitializeChildren(node);
            node.ListView.Refresh(data);
            value && OnNodeExpanding(node);
        };

        /*
        * Method Description:
        * Finish refresh list: prompts user that system has loaded.
        *
        * Parameters:
        * value: The value of the node that finish to refresh.
        */
        this.FinishRefresh = function (value) {
            var node = value ? FindNode(value) : m_root;
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            InitializeChildren(node);
            node.ListView.FinishRefresh();
        };

        /*
        * Method Description:
        * Expands the node asscoated with the specified value.
        *
        * Parameters:
        * value: The value of the node will be expanded.
        */
        this.Expand = function (value) {
            var node = FindNode(value);
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            OnNodeExpanding(node);
        };

        /*
        * Method Description:
        * Collapses the node asscoated with the specified value.
        *
        * Parameters:
        * value: The value of the node will be collapsed.
        */
        this.Collapse = function (value) {
            var node = FindNode(value);
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            OnNodeCollapsing(node);
        };

        /*
        * Method Description:
        * Expands or collapses the node asscoated with the specified value.
        *
        * Parameters:
        * value: The value of the node will be expanded or collapsed.
        */
        this.ExpandOrCollapse = function (value) {
            var node = FindNode(value);
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            if (node.IsExpanded) {
                OnNodeCollapsing(node);
            } else {
                OnNodeExpanding(node);
            }
        };

        /*
        * Method Description:
        * Append value to the node asscoated with the specified value.
        *
        * Parameters:
        * current: The value of the current node.
        * value: The value to append to this tree.
        */
        this.Append = function (current, value) {
            var node = current ? FindNode(current) : m_root;
            if (!node) {
                throw new Error("Can not find node by the specified value.");
            }

            InitializeChildren(node);
            node.ListView.Append(value);
            OnNodeExpanding(node);
        };

        /*
        * Method Description:
        * Updates the node asscoated with the specified value.
        *
        * Parameters:
        * value: The value of the node to update.
        */
        this.Update = function (value) {
            var node = FindNode(value);
            if (!node) {
                return;
            }

            node.Value = value;
            node.Parent.ListView.Update(value);
        };

        /*
        * Method Description:
        * Removes the node asscoated with the specified value.
        *
        * Parameters:
        * value: The value of the node will be removed.
        */
        this.Remove = function (value) {
            var node = FindNode(value);
            if (!node) {
                return;
            }

            RemoveByParent(node);
            m_nodes.Remove(node);

            var parent = node.Parent;
            parent.ListView.Remove(value);
            if (parent.ListView.GetSelectedIndex() < 0) {
                if (parent.Parent) {
                    parent.Parent.ListView.SetSelectedValue(parent.Value);
                } else if (parent.ListView.GetItemsCount() > 0) {
                    parent.ListView.SetSelectedIndex(0);
                }
            }
        };

        /*
        * Method Description:
        * Gets the current selected value.
        */
        this.GetSelectedValue = function () {
            var node = FindSelectedNode();
            return node ? node.Value : null;
        };

        /*
        * Method Description:
        * Sets the node associated with the specified value to selected.
        */
        this.SetSelectedValue = function (value) {
            var node = FindNode(value);
            if (node) {
                node.Parent.ListView.SetSelectedValue(node.Value);
            } else {
                var current = FindSelectedNode();
                if (current) {
                    current.Parent.ListView.SetSelectedIndex(-1);
                }
            }
        };

        /*
        * Method Description:
        * Gets the ChildrenCreating event which occurs when creating the child nodes list.
        * The argument object passed to event handlers as follows:
        * {Value: nodeObject, ListParameters: listViewParameters}.
        */
        this.GetChildrenCreatingEvent = function () {
            return m_childrenCreating;
        };

        /*
        * Method Description:
        * Gets the NodeCreated event which occurs after creating a node.
        * The argument object passed to event handlers as follows:
        * {Value: nodeObject, Node: htmlNode}.
        */
        this.GetNodeCreatedEvent = function () {
            return m_nodeCreated;
        };

        /*
        * Method Description:
        * Gets the NodeExpanding event which occurs when a node is expanding.
        * The argument object passed to event handlers as follows:
        * {Value: nodeObject, Node: htmlNode, IsInitialized: whether the children list has been initialized}.
        */
        this.GetNodeExpandingEvent = function () {
            return m_nodeExpanding;
        };

        /*
        * Method Description:
        * Gets the NodeCollapsing event which occurs when a node is collapsing.
        * The argument object passed to event handlers as follows:
        * {Value: nodeObject, Node: htmlNode}.
        */
        this.GetNodeCollapsingEvent = function () {
            return m_nodeCollapsing;
        };

        /*
        * Method Description:
        * Gets the SelectedChanged event which occurs when the selected node has changed.
        * The argument object passed to event handlers as follows:
        * {Value: nodeObject, Node: htmlNode, OtherNodes: otherHtmlNodes}.
        */
        this.GetSelectedChangedEvent = function () {
            return m_selectedChanged;
        };
    };

    /***************** ProgressBar Class Definition *****************/

    /*
    * Class Description:
    * Represents a progress bar, progress value is between 0 and 100.
    *
    * Constructor Parameters:
    *
    * parameters: The parameters for constructor which is a object as follow:
    * {
    *   OnCreateBar: The callback function when creating the progress bar element.
    *   OnCreateStrip: The callback function when creating the progress strip element.
    *   OnCreateTip: The callback function when creating the text tip element.
    * }
    */
    var ProgressBar = Xphter.ProgressBar = function (parameters) {
        var m_barElement = null;
        var m_stripElement = null;
        var m_tipElement = null;

        var m_onCreateBar = null;
        var m_onCreateStrip = null;
        var m_onCreateTip = null;

        var m_currentProgress = 0;
        var m_progressChanged = new Event(this, "ProgressChanged");

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters of progress bar is undefined.");
            }

            //analyze parameters
            DataTypeIdentity.IsFunction(parameters.OnCreateBar) && (m_onCreateBar = parameters.OnCreateBar);
            DataTypeIdentity.IsFunction(parameters.OnCreateStrip) && (m_onCreateStrip = parameters.OnCreateStrip);
            DataTypeIdentity.IsFunction(parameters.OnCreateTip) && (m_onCreateTip = parameters.OnCreateTip);

            //create elements
            m_barElement = DocumentUtility.CreateElement("div", null, {
                width: "100%",
                height: "100%",
                margin: "0px",
                padding: "0px",
                position: "relative",
                left: "0px",
                top: "0px",
                overflow: "hidden",
                border: "1px solid #777777",
                backgroundColor: "#FFFFFF",
                textAlign: "left"
            });
            m_onCreateBar && m_onCreateBar(m_barElement);

            m_stripElement = DocumentUtility.CreateElement("div", null, {
                width: "0%",
                height: "100%",
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                backgroundColor: "#A0C5E8"
            });
            m_onCreateStrip && m_onCreateStrip(m_stripElement);
            m_barElement.appendChild(m_stripElement);

            m_tipElement = DocumentUtility.CreateElement("div", null, {
                width: "100%",
                height: "100%",
                margin: "0px",
                padding: "0px",
                borderStyle: "none",
                position: "absolute",
                left: "0px",
                top: "0px",
                backgroundColor: "transparent",
                color: "#FF0000",
                fontWeight: "bold",
                textAlign: "center",
                lineHeight: "100%"
            }, "0%");
            m_onCreateTip && m_onCreateTip(m_tipElement);
            m_barElement.appendChild(m_tipElement);
        }

        /*
        * Method Description:
        * Sets the progress value which must between 0 and 100.
        */
        this.SetProgressValue = function (value, text) {
            if (global.isNaN(value)) {
                throw new Error("Progress value is not a number.");
            }

            var progress = Math.max(0, Math.min(100, value - 0));
            if (m_currentProgress != progress) {
                m_currentProgress = progress;
                m_stripElement.style.width = m_currentProgress + "%";
                m_progressChanged.Raise(m_currentProgress);
            }
            this.SetTipText(text);
        };

        /*
        * Method Description:
        * Sets the tip text.
        */
        this.SetTipText = function (text) {
            var _text = (text || String.Empty) + String.Empty;
            m_tipElement.innerHTML = HtmlEncode(_text || (m_currentProgress.toFixed(DataTypeIdentity.IsInteger(m_currentProgress) ? 0 : 2) + "%"));
        };

        /*
        * Method Description:
        * Gets current progress value.
        */
        this.GetProgressValue = function () {
            return m_currentProgress;
        };

        /*
        * Method Description:
        * Gets this ProgressChanged event, the new progress value will pass to event handlers.
        */
        this.GetProgressChangedEvent = function () {
            return m_progressChanged;
        };

        /*
        * Method Description:
        * Gets the progress bar HTML element.
        */
        this.GetBarElement = function () {
            return m_barElement;
        };

        /*
        * Method Description:
        * Replaces a existing element with this control.
        * 
        * Parameters:
        * element: The element will be replaced.
        */
        this.Replace = function (element) {
            element && element.parentNode && element.parentNode.replaceChild(m_barElement, element);
        };

        /*
        * Method Description:
        * Places this control in the specified element.
        * 
        * Parameters:
        * element: The element will contain this control.
        */
        this.Place = function (element) {
            element && element.appendChild && element.appendChild(m_barElement);
        };
    };

    /***************** RollingImage Class Definition *****************/

    /*
    * Class Description:
    * Represents a rolling image.
    * It show a big image initially, then it start to become short and show a small image in the end.
    *
    * Constructor Parameters:
    *
    * parameters: The parameters for constructor which is a object as follow:
    * {
    *   Title: The title of these images.
    *   BigImageUri: The big image URI.
    *   SmallImageUri: The small image URI.
    *   Uri: The target uri of big image and small image.
    *   DisplayDuration: The duration of displaying big image in milliseconds.
    *   SlidingDuration: The duration of becoming to small image from big image in milliseconds.
    * }
    */
    var RollingImage = Xphter.RollingImage = function (parameters) {
        var m_title = String.Empty;
        var m_bigImageUri = null;
        var m_smallImageUri = null;
        var m_uri = String.Empty;
        var m_displayDuration = 3000;
        var m_slidingDuration = 1000;

        var m_bigImage = null;
        var m_smallImage = null;
        var m_rollingElement = null;

        var m_bigImageHeight = 0;
        var m_smallImageHeight = 0;

        var m_canSwitch = false;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Parameters of RollingImage is undefined.");
            }

            //analyze parameters
            parameters.Title && (m_title = parameters.Title + String.Empty);
            if (!(parameters.BigImageUri && (m_bigImageUri = parameters.BigImageUri + String.Empty))) {
                throw new Error("Big image of RollingImage is undefined.");
            }
            if (!(parameters.SmallImageUri && (m_smallImageUri = parameters.SmallImageUri + String.Empty))) {
                throw new Error("Small image of RollingImage is undefined.");
            }
            parameters.Uri && (m_uri = parameters.Uri + String.Empty);
            !global.isNaN(parameters.DisplayDuration) && (m_displayDuration = (parameters.DisplayDuration - 0 || m_displayDuration));
            !global.isNaN(parameters.SlidingDuration) && (m_slidingDuration = (parameters.SlidingDuration - 0 || m_slidingDuration));

            //create elements
            m_smallImage = DocumentUtility.CreateElement("img", {
                src: m_smallImageUri,
                alt: m_title,
                title: m_title
            });
            EventUtility.Register(m_smallImage, "mouseover", function (e) {
                if (!m_canSwitch) {
                    return;
                }

                m_rollingElement.removeChild(m_smallImage);
                m_rollingElement.appendChild(m_bigImage);
            });

            m_bigImage = DocumentUtility.CreateElement("img", {
                src: m_bigImageUri,
                alt: m_title,
                title: m_title
            });
            EventUtility.Register(m_bigImage, "mouseout", function (e) {
                if (!m_canSwitch) {
                    return;
                }

                m_rollingElement.removeChild(m_bigImage);
                m_rollingElement.appendChild(m_smallImage);
            });

            m_rollingElement = DocumentUtility.CreateElement("div", null, {
                margin: "0px",
                padding: "0px",
                cursor: "pointer",
                overflow: "hidden"
            }, m_bigImage);
            EventUtility.Register(m_rollingElement, "click", function (e) {
                global.open(m_uri, "_blank");
            });
        }

        /*
        * Roll these images.
        */
        function Roll() {
            if (m_bigImage.height && m_smallImage.height) {
                var speed = 0;
                /**************************************
                * Get image height, in IE9 image height will be zero when remove it from it's parent element.
                ***************************************/
                m_bigImageHeight = m_bigImage.height;
                m_smallImageHeight = m_smallImage.height;
                new Animator({
                    Fps: 60,
                    Duration: m_slidingDuration,
                    OnInitialize: function () {
                        speed = (m_smallImageHeight - m_bigImageHeight) / m_slidingDuration;
                    },
                    OnFrame: function (elapsedTime) {
                        m_rollingElement.style.height = (m_bigImageHeight + Math.floor(speed * elapsedTime)) + "px";
                    },
                    OnDispose: function () {
                        m_rollingElement.removeChild(m_bigImage);
                        m_rollingElement.appendChild(m_smallImage);
                        m_rollingElement.style.height = "auto";
                        m_canSwitch = true;
                    }
                }).Start();
            } else {
                global.setTimeout(Roll, 1);
            }
        }

        /*
        * Method Description:
        * Start to roll image.
        */
        this.StartRoll = function () {
            global.setTimeout(Roll, m_displayDuration);
        };

        /*
        * Method Description:
        * Gets HTML element of this control.
        */
        this.GetRollingElement = function () {
            return m_rollingElement;
        };

        /*
        * Method Description:
        * Replaces a existing element with this control.
        * 
        * Parameters:
        * element: The element will be replaced.
        */
        this.Replace = function (element) {
            element && element.parentNode && element.parentNode.replaceChild(m_rollingElement, element);
        };

        /*
        * Method Description:
        * Places this control in the specified element.
        * 
        * Parameters:
        * element: The element will contain this control.
        */
        this.Place = function (element) {
            element && element.appendChild && element.appendChild(m_rollingElement);
        };
    };

    /***************** RestrictedTextBox Class Definition *****************/

    /*
    * Class Description:
    * Use a predicate to restrict input of a TextBox.
    * The predicate can be a function or a string or a regular expression.
    * This class can not check none-ascii characters.
    *
    * Constructor Parameters:
    *
    * predicate: A function or a regular expression pattern or a RegExp object.
    * target: The target TextBox element.
    */
    var RestrictedTextBox = Xphter.RestrictedTextBox = function (predicate, target) {
        var m_predicate = null;
        var m_target = null;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!target) {
                throw new Error("Target input text is null.");
            }

            //initialize
            if (DataTypeIdentity.IsFunction(predicate)) {
                m_predicate = predicate;
            } else if (predicate instanceof RegExp) {
                m_predicate = function (text) {
                    predicate.lastIndex = 0;
                    return predicate.test(text);
                };
            } else if (DataTypeIdentity.IsString(predicate)) {
                if (predicate + String.Empty) {
                    var regex = new RegExp(predicate + String.Empty, "gi");
                    m_predicate = function (text) {
                        regex.lastIndex = 0;
                        return regex.test(text);
                    };
                }
            }
            m_target = target;

            //register event handler
            if (m_predicate) {
                EventUtility.Register(m_target, "keypress", function (e) {
                    var charCode = DataTypeIdentity.IsUndefined(e.charCode) ? e.keyCode : e.charCode;
                    if (!charCode || charCode < 32) {
                        return;
                    }

                    !m_predicate(m_target.value + String.fromCharCode(charCode)) && e.preventDefault();
                }, false);
            }
        }
    };

    /*
    * Restrict the input of a TextBox to integer.
    */
    Xphter.RestrictedTextBox.AsInteger = function (target) {
        new Xphter.RestrictedTextBox("^[\\-\\+]?(?:0x)?$|^[\\-\\+]?(?:0x)?\\d+$", target);
    };

    /*
    * Restrict the input of a TextBox to number.
    */
    Xphter.RestrictedTextBox.AsNumber = function (target) {
        new Xphter.RestrictedTextBox("^[\\-\\+]?(?:0x)?$|^[\\-\\+]?(?:0x)?\\d+(?:\\.\\d*)?$", target);
    };

    /***************** Calendar Class Definition *****************/

    /*
    * Class Description:
    * Represents a calendar which attach a input text box.
    *
    * Constructor Parameters:
    *
    * parameters: The parameters for this calendar which is a object as follow:
    * {
    *   Target: The target input text box.
    *   Time: The original time.
    *   IsShowTime: Whether show hour, minute and second info.
    *   IsCloseAfterChoise: Whether close this picker after choosing a datetime.
    * }
    */
    var Calendar = Xphter.Calendar = function (parameters) {
        //size arguments
        var m_headerHeight = 20;
        var m_arrowWidth = 20;
        var m_tableWidth = 250;
        var m_margin = 10;

        //gloabl color
        var m_backgroundColor = "#F7F7F7";
        var m_borderColor = "#AAAAAA";

        //header options
        var m_headerBackgroundColor = "#E9F1FA";
        var m_headerFontSize = 12;
        var m_headerButtonBorderColor = "#738899";
        var m_headerButtonBackgroundColor = "#FFFFFF";
        var m_headerButtonTextColor = "#738899";

        //week header options
        var m_weekBackgroundColor = "#E9F1FA";
        var m_weekTextColor = "#999999";
        var m_weekFontSize = 12;

        //time table options
        var m_timeBackgroundColor = "#E9F1FA";
        var m_timeTextColor = "#000000";
        var m_timeFontSize = 14;

        //day table options
        var m_dayRowHeight = 25;
        var m_dayTextColor = "#333333";
        var m_dayOtherTextColor = "#777777";
        var m_dayHoverTextColor = "#FF6600";
        var m_daySelectedBackgroundColor = "#FF9900";
        var m_dayFontSize = 14;

        //month table options
        var m_monthRowHeight = 40;
        var m_monthTextColor = "#000000";
        var m_monthHoverTextColor = "#FF6600";
        var m_monthSelectedBackgroundColor = "#FF9900";
        var m_monthFontSize = 14;

        //year table options
        var m_yearRowHeight = 40;
        var m_yearTextColor = "#000000";
        var m_yearHoverTextColor = "#FF6600";
        var m_yearSelectedBackgroundColor = "#FF9900";
        var m_yearFontSize = 14;

        //hour table options
        var m_hourRowHeight = 32;
        var m_hourTextColor = "#000000";
        var m_hourHoverTextColor = "#FF6600";
        var m_hourSelectedBackgroundColor = "#FF9900";
        var m_hourFontSize = 14;

        //minute table options
        var m_minuteRowHeight = 25;
        var m_minuteTextColor = "#000000";
        var m_minuteHoverTextColor = "#FF6600";
        var m_minuteSelectedBackgroundColor = "#FF9900";
        var m_minuteFontSize = 14;

        //second table options
        var m_secondRowHeight = 25;
        var m_secondTextColor = "#000000";
        var m_secondHoverTextColor = "#FF6600";
        var m_secondSelectedBackgroundColor = "#FF9900";
        var m_secondFontSize = 14;

        //HTML elements
        var m_pickerElement = null;
        var m_target = null;
        var m_iframeElement = null;

        var m_dayPanel = null;
        var m_dayHeader = null;
        var m_dayTable = null;
        var m_dayTimeTable = null;

        var m_monthPanel = null;
        var m_monthHeader = null;
        var m_monthTable = null;

        var m_yearPanel = null;
        var m_yearHeader = null;
        var m_yearTable = null;

        var m_hourPanel = null;
        var m_hourHeader = null;
        var m_hourTable = null;

        var m_minutePanel = null;
        var m_minuteHeader = null;
        var m_minuteTable = null;

        var m_secondPanel = null;
        var m_secondHeader = null;
        var m_secondTable = null;

        var m_panels = null;

        //current display time
        var m_time = null;
        var m_hasTime = true;
        var m_timeChanged = new Event(this, "TimeChanged");

        //datetime picker options
        var m_isShown = false;
        var m_isShowTime = false;
        var m_isCloseAfterChoise = false;

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("Calendar parameters is undefined.");
            }
            if (!DataTypeIdentity.IsNotNullObject(parameters.Target)) {
                throw new Error("The target input element is undefined.");
            }

            //analyze parameters
            m_target = parameters.Target;
            m_target.readOnly = true;
            !DataTypeIdentity.IsUndefined(parameters.IsShowTime) && (m_isShowTime = !!parameters.IsShowTime);
            !DataTypeIdentity.IsUndefined(parameters.IsCloseAfterChoise) && (m_isCloseAfterChoise = !!parameters.IsCloseAfterChoise);

            //create elements of picker
            m_pickerElement = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + m_margin * 2 + "px",
                height: "auto",
                margin: "0px",
                padding: "0px",
                borderStyle: "solid",
                borderWidth: "1px",
                borderColor: m_borderColor,
                position: "absolute",
                left: "0px",
                top: "0px",
                zIndex: "99",
                display: "none",
                backgroundColor: m_backgroundColor
            });
            global.document.body.appendChild(m_pickerElement);

            //avoid a bug in IE6: SELECT element cover all DIV elements.
            if (true || BrowserCapability.IsIE && BrowserCapability.MajorVersion == 6) {
                m_iframeElement = DocumentUtility.CreateElement("iframe", {
                    width: (m_tableWidth + m_margin * 2) + "px",
                    height: "1px",
                    frameborder: "0",
                    marginwidth: "0px",
                    marginheight: "0px"
                }, {
                    borderStyle: "none",
                    margin: "0px",
                    padding: "0px",
                    position: "absolute",
                    left: "0px",
                    top: "0px",
                    zIndex: "98",
                    display: "none",
                    width: (m_tableWidth + m_margin * 2) + "px",
                    backgroundColor: "#FF00FF"
                });
                global.document.body.appendChild(m_iframeElement);
            }

            CreateDayPanel();
            CreateMonthPanel();
            CreateYearPanel();
            CreateHourPanel();
            CreateMinutePanel();
            CreateSecondPanel();
            m_panels = [m_dayPanel, m_monthPanel, m_yearPanel, m_hourPanel, m_minutePanel, m_secondPanel];
            for (var i = 0; i < m_panels.length; i++) {
                m_pickerElement.appendChild(m_panels[i]);
            }

            var buttonNow = null;
            var buttonClear = null;
            var buttonOK = null;
            var buttonCancel = null;
            m_pickerElement.appendChild(DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: "0px " + m_margin + "px",
                padding: "0px"
            }, [buttonNow = DocumentUtility.CreateElement("button", null, {
                margin: "0px",
                marginBottom: m_margin + "px",
                cssFloat: "left",
                styleFloat: "left"
            }, "Now"), buttonClear = DocumentUtility.CreateElement("button", null, {
                margin: "0px",
                marginLeft: m_margin + "px",
                marginBottom: m_margin + "px",
                cssFloat: "left",
                styleFloat: "left"
            }, "Clear"), buttonCancel = DocumentUtility.CreateElement("button", null, {
                margin: "0px",
                marginBottom: m_margin + "px",
                cssFloat: "right",
                styleFloat: "right"
            }, "Cancel"), buttonOK = DocumentUtility.CreateElement("button", null, {
                margin: "0px",
                marginRight: m_margin + "px",
                marginBottom: m_margin + "px",
                cssFloat: "right",
                styleFloat: "right"
            }, "OK"), DocumentUtility.CreateClearBoth()]));

            //register event handlers
            EventUtility.Register(buttonNow, "click", function () {
                SetTimeValue(new Date());
                if (m_isCloseAfterChoise) {
                    Close();
                } else {
                    RefereshDayPanel();
                    ShowPanel(m_dayPanel);
                }
            }, false);
            EventUtility.Register(buttonClear, "click", function () {
                ClearTime();
            }, false);
            EventUtility.Register(buttonOK, "click", function () {
                Ensure();
            }, false);
            EventUtility.Register(buttonCancel, "click", function () {
                Close();
            }, false);
            EventUtility.Register(m_target, "click", function () {
                Show();
            }, false);
            EventUtility.Register(global.document.body, "click", function (e) {
                var target = e.target;
                if (target == m_target || target == m_pickerElement || DocumentUtility.IsChild(target, m_pickerElement)) {
                    return;
                }

                Close();
            }, false);

            if (parameters.Time === null) {
                m_time = new Date();
                ClearTime();
            } else {
                SetTimeValue(parameters.Time instanceof Date ? parameters.Time : new Date());
            }
        }

        /*
        * Show the specified panel.
        */
        function ShowPanel(panel) {
            for (var i = 0; i < m_panels.length; i++) {
                m_panels[i].style.display = "none";
            }
            panel.style.display = String.Empty;
        }

        /*
        * Set m_time to the specified value.
        */
        function SetTimeValue(time) {
            time = (time instanceof Date ? time : new Date());
            if (time.Equals(m_time)) {
                return;
            }

            var year = time.getFullYear();
            var month = time.getMonth();
            var date = time.getDate();
            var hour = time.getHours();
            var minute = time.getMinutes();
            var second = time.getSeconds();

            //update time
            m_time = new Date(year, month, date, hour, minute, second);
            m_hasTime = true;

            //update target
            var value = m_time.getFullYear() + "/" + (month < 9 ? "0" : String.Empty) + (month + 1) + "/" + (date < 10 ? "0" : String.Empty) + date;
            if (m_isShowTime) {
                value += " " + (hour < 10 ? "0" : String.Empty) + hour + ":" + (minute < 10 ? "0" : String.Empty) + minute + ":" + (second < 10 ? "0" : String.Empty) + second;
            }
            m_target.value = value;

            m_timeChanged.Raise();
        }

        /*
        * Set display time to the specified time.
        */
        function SetTime(time) {
            // update time value
            if (DataTypeIdentity.IsUndefinedOrNull(time)) {
                ClearTime();
            } else {
                SetTimeValue(time);
            }

            //update picker element
            if (m_isShown) {
                RefereshDayPanel();
                ShowPanel(m_dayPanel);
            }
        }

        /*
        * Clear display time.
        */
        function ClearTime() {
            if (!m_hasTime) {
                return;
            }

            m_hasTime = false;
            m_target.value = String.Empty;
            m_timeChanged.Raise();

            m_isShown && m_isCloseAfterChoise && Close();
        }

        /*
        * Show picker.
        */
        function Show() {
            var x = DocumentUtility.GetElementX(m_target) + BrowserGeometry.GetHorizontalScroll();
            var y = DocumentUtility.GetElementY(m_target) + m_target.offsetHeight + BrowserGeometry.GetVerticalScroll();

            if (m_isShown) {
                if (m_iframeElement) {
                    m_iframeElement.style.left = x + "px";
                    m_iframeElement.style.top = y + "px";
                }

                m_pickerElement.style.left = x + "px";
                m_pickerElement.style.top = y + "px";
                return;
            }

            m_isShown = true;
            RefereshDayPanel();
            ShowPanel(m_dayPanel);

            if (m_iframeElement) {
                m_iframeElement.style.left = x + "px";
                m_iframeElement.style.top = y + "px";
                m_iframeElement.style.display = String.Empty;
                global.setTimeout(function () {
                    m_iframeElement.height = m_pickerElement.offsetHeight + "px";
                    m_iframeElement.style.height = m_pickerElement.offsetHeight + "px";
                }, 0);
            }

            m_pickerElement.style.left = x + "px";
            m_pickerElement.style.top = y + "px";
            m_pickerElement.style.display = "block";
        }

        /*
        * Close picker.
        */
        function Close() {
            m_isShown = false;
            m_iframeElement && (m_iframeElement.style.display = "none");
            m_pickerElement.style.display = "none";
        }

        /*
        * Close picker and accept current display time.
        */
        function Ensure() {
            Close();
            SetTime(m_time);
        }

        /*
        * Create a table element.
        */
        function CreateTable(rowCount, columnCount, width, rowHeight) {
            var table = DocumentUtility.CreateElement("table", {
                border: "0",
                cellpadding: "0",
                cellspacing: "0"
            }, {
                width: width + "px",
                height: "auto",
                margin: "0px",
                padding: "0px"
            });

            var row = null;
            var cell = null;
            for (var i = 0; i < rowCount; i++) {
                row = table.insertRow(i);
                row.style.height = rowHeight + "px";

                for (var j = 0; j < columnCount; j++) {
                    cell = row.insertCell(j);
                    cell.align = "center";
                    cell.vAlign = "middle";
                }
            }

            return table;
        }

        /*
        * Fill the specified table.
        */
        function FillTable(table, styles, items) {
            var index = 0;
            var row = null;
            var cell = null;
            var item = null;

            for (var member in styles) {
                table.style[member] = styles[member] || String.Empty;
            }
            for (var i = 0; i < table.rows.length; i++) {
                if (index >= items.length) {
                    break;
                }

                row = table.rows[i];

                for (var j = 0; j < row.cells.length; j++) {
                    if (index >= items.length) {
                        break;
                    }

                    cell = row.cells[j];
                    item = items[index++];

                    cell.innerHTML = item.text || String.Empty;
                    cell.tag = item.tag;
                    if (item.styles) {
                        for (var member in item.styles) {
                            cell.style[member] = item.styles[member] || String.Empty;
                        }
                    }
                    if (item.attributes) {
                        for (var member in item.attributes) {
                            cell[member] = item.attributes[member] || null;
                        }
                    }
                }
            }

            return table;
        }

        /*
        * Create the panel used to show days.
        */
        function CreateDayPanel() {
            m_dayPanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_dayPanel.appendChild(m_dayHeader = CreateTable(1, 4, m_tableWidth, m_headerHeight));
            m_dayPanel.appendChild(FillTable(CreateTable(1, 7, m_tableWidth, m_headerHeight), {
                backgroundColor: m_weekBackgroundColor,
                fontSize: m_weekFontSize + "px",
                color: m_weekTextColor
            }, [{
                text: Date.WeekdayShortNames[1]
            }, {
                text: Date.WeekdayShortNames[2]
            }, {
                text: Date.WeekdayShortNames[3]
            }, {
                text: Date.WeekdayShortNames[4]
            }, {
                text: Date.WeekdayShortNames[5]
            }, {
                text: Date.WeekdayShortNames[6]
            }, {
                text: Date.WeekdayShortNames[0]
            }]));
            m_dayPanel.appendChild(m_dayTable = CreateTable(6, 7, m_tableWidth, m_dayRowHeight));
            m_dayPanel.appendChild(m_dayTimeTable = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            !m_isShowTime && (m_dayTimeTable.style.display = "none");
        }

        /*
        * Referesh day panel.
        */
        function RefereshDayPanel() {
            var days = [];
            var year = m_time.getFullYear();
            var month = m_time.getMonth();
            var date = m_time.getDate();
            var hour = m_time.getHours();
            var minute = m_time.getMinutes();
            var second = m_time.getSeconds();
            var firstDay = new Date(year, month, 1, hour, minute, second);

            //create previous month days
            var previousYear = year - (month == 0 ? 1 : 0);
            var previousMonth = month == 0 ? 11 : (month - 1);
            var previousCount = firstDay.getDay() == 0 ? 6 : firstDay.getDay() - 1;
            var previousMonthDayCount = Date.GetDayCount(previousYear, previousMonth);
            for (var i = previousCount - 1; i >= 0; i--) {
                days.push(new Date(previousYear, previousMonth, previousMonthDayCount - i, hour, minute, second));
            }

            //create current month days
            var currentCount = Date.GetDayCount(year, month);
            for (var i = 0; i < currentCount; i++) {
                days.push(new Date(year, month, i + 1, hour, minute, second));
            }

            //create next month days
            var nextYear = year + (month == 11 ? 1 : 0);
            var nextMonth = month == 11 ? 0 : (month + 1);
            var nextCount = 42 - days.length;
            for (var i = 0; i < nextCount; i++) {
                days.push(new Date(nextYear, nextMonth, i + 1));
            }

            //fill day header
            FillTable(m_dayHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Previous month",
                    onclick: function () {
                        var year = m_time.getFullYear();
                        var month = m_time.getMonth();
                        var date = m_time.getDate();
                        var time = new Date(month == 0 ? year - 1 : year, month == 0 ? 11 : month - 1, 1, m_time.getHours(), m_time.getMinutes(), m_time.getSeconds());
                        time.setDate(Math.min(Date.GetDayCount(m_time.getFullYear(), m_time.getMonth()), date));

                        SetTimeValue(time);
                        RefereshDayPanel();
                    }
                }
            }, {
                text: Date.MonthNames[month],
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose month",
                    onclick: function () {
                        RefereshMonthPanel();
                        ShowPanel(m_monthPanel);
                    }
                }
            }, {
                text: year,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose year",
                    onclick: function () {
                        var year = m_time.getFullYear();
                        RefereshYearPanel(year - 6, year + 5);
                        ShowPanel(m_yearPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Next month",
                    onclick: function () {
                        var year = m_time.getFullYear();
                        var month = m_time.getMonth();
                        var date = m_time.getDate();
                        var time = new Date(month == 11 ? year + 1 : year, month == 11 ? 0 : month + 1, 1, m_time.getHours(), m_time.getMinutes(), m_time.getSeconds());
                        time.setDate(Math.min(Date.GetDayCount(m_time.getFullYear(), m_time.getMonth()), date));

                        SetTimeValue(time);
                        RefereshDayPanel();
                    }
                }
            }]);

            //fill day table
            var items = [];
            var day = null;
            for (var i = 0; i < days.length; i++) {
                day = days[i];

                items.push({
                    text: day.getDate() + String.Empty,
                    tag: day,
                    styles: {
                        backgroundColor: i >= previousCount && i < previousCount + currentCount && day.getDate() == date ? m_daySelectedBackgroundColor : String.Empty,
                        color: i < previousCount || i >= previousCount + currentCount ? m_dayOtherTextColor : m_dayTextColor,
                        fontWeight: i >= previousCount && i < previousCount + currentCount ? "bold" : String.Empty,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_dayHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTimeValue(this.tag);
                            if (m_isCloseAfterChoise) {
                                Close();
                            } else {
                                var row = null;
                                for (var i = 0; i < m_dayTable.rows.length; i++) {
                                    row = m_dayTable.rows[i];
                                    for (var j = 0; j < row.cells.length; j++) {
                                        row.cells[j].style.backgroundColor = String.Empty;
                                    }
                                }
                                this.style.backgroundColor = m_daySelectedBackgroundColor;
                            }
                        }
                    }
                });
            }
            FillTable(m_dayTable, {
                fontSize: m_dayFontSize + "px"
            }, items);

            //fill time header
            FillTable(m_dayTimeTable, {
                backgroundColor: m_timeBackgroundColor,
                color: m_timeTextColor,
                fontSize: m_timeFontSize
            }, [{
                text: hour + " h",
                tag: hour,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose hour",
                    onclick: function () {
                        RefereshHourPanel();
                        ShowPanel(m_hourPanel);
                    }
                }
            }, {
                text: minute + " m",
                tag: minute,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose minute",
                    onclick: function () {
                        RefereshMinutePanel();
                        ShowPanel(m_minutePanel);
                    }
                }
            }, {
                text: second + " s",
                tag: second,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose second",
                    onclick: function () {
                        RefereshSecondPanel();
                        ShowPanel(m_secondPanel);
                    }
                }
            }]);
        }

        /*
        * Create the panel used to show months.
        */
        function CreateMonthPanel() {
            m_monthPanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_monthPanel.appendChild(m_monthHeader = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            m_monthPanel.appendChild(m_monthTable = CreateTable(3, 4, m_tableWidth, m_monthRowHeight));
        }

        /*
        * Referesh month panel.
        */
        function RefereshMonthPanel() {
            var month = m_time.getMonth();

            //fill month header
            FillTable(m_monthHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Previous month",
                    onclick: function () {
                        var year = m_time.getFullYear();
                        var month = m_time.getMonth();
                        var date = m_time.getDate();
                        if (month > 0) {
                            var time = new Date(year, month - 1, 1, m_time.getHours(), m_time.getMinutes(), m_time.getSeconds());
                            time.setDate(Math.min(Date.GetDayCount(year, month - 1), date));

                            SetTimeValue(time);
                            RefereshMonthPanel();
                        }
                    }
                }
            }, {
                text: Date.MonthNames[month],
                tag: month,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose date",
                    onclick: function () {
                        RefereshDayPanel();
                        ShowPanel(m_dayPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Next month",
                    onclick: function () {
                        var year = m_time.getFullYear();
                        var month = m_time.getMonth();
                        var date = m_time.getDate();
                        if (month < 11) {
                            var time = new Date(year, month + 1, 1, m_time.getHours(), m_time.getMinutes(), m_time.getSeconds());
                            time.setDate(Math.min(Date.GetDayCount(year, month + 1), date));

                            SetTimeValue(time);
                            RefereshMonthPanel();
                        }
                    }
                }
            }]);

            //fill month table
            var items = [];
            for (var i = 0; i < 12; i++) {
                items.push({
                    text: Date.MonthNames[i],
                    tag: i,
                    styles: {
                        backgroundColor: i == month ? m_monthSelectedBackgroundColor : String.Empty,
                        color: m_monthTextColor,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_monthHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTime(new Date(m_time.getFullYear(), this.tag, Math.min(m_time.getDate(), Date.GetDayCount(m_time.getFullYear(), this.tag)), m_time.getHours(), m_time.getMinutes(), m_time.getSeconds()));
                        }
                    }
                });
            }
            FillTable(m_monthTable, {
                fontSize: m_monthFontSize + "px"
            }, items);
        }

        /*
        * Create the panel used to show years.
        */
        function CreateYearPanel() {
            m_yearPanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_yearPanel.appendChild(m_yearHeader = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            m_yearPanel.appendChild(m_yearTable = CreateTable(3, 4, m_tableWidth, m_yearRowHeight));
        }

        /*
        * Referesh year panel.
        */
        function RefereshYearPanel(minYear, maxYear) {
            var year = m_time.getFullYear();

            //fill year header
            FillTable(m_yearHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Forward",
                    onclick: function () {
                        RefereshYearPanel(minYear - 12, maxYear - 12);
                    }
                }
            }, {
                text: minYear + " - " + maxYear,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose date",
                    onclick: function () {
                        ShowPanel(m_dayPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Backward",
                    onclick: function () {
                        RefereshYearPanel(minYear + 12, maxYear + 12);
                    }
                }
            }]);

            //fill year table
            var items = [];
            for (var i = minYear; i <= maxYear; i++) {
                items.push({
                    text: i + String.Empty,
                    tag: i,
                    styles: {
                        backgroundColor: i == year ? m_yearSelectedBackgroundColor : String.Empty,
                        color: m_yearTextColor,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_monthHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTime(new Date(this.tag, m_time.getMonth(), Math.min(Date.GetDayCount(this.tag, m_time.getMonth()), m_time.getDate()), m_time.getHours(), m_time.getMinutes(), m_time.getSeconds()));
                        }
                    }
                });
            }
            FillTable(m_yearTable, {
                fontSize: m_yearFontSize + "px"
            }, items);
        }

        /*
        * Create the panel used to show hours.
        */
        function CreateHourPanel() {
            m_hourPanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_hourPanel.appendChild(m_hourHeader = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            m_hourPanel.appendChild(m_hourTable = CreateTable(4, 6, m_tableWidth, m_hourRowHeight));
        }

        /*
        * Referesh hour panel.
        */
        function RefereshHourPanel() {
            var hour = m_time.getHours();

            //fill hour header
            FillTable(m_hourHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Previous hour",
                    onclick: function () {
                        var hour = m_time.getHours();
                        if (hour > 0) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), hour - 1, m_time.getMinutes(), m_time.getSeconds()));
                            RefereshHourPanel();
                        }
                    }
                }
            }, {
                text: hour + " h",
                tag: hour,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose date",
                    onclick: function () {
                        RefereshDayPanel();
                        ShowPanel(m_dayPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Next hour",
                    onclick: function () {
                        var hour = m_time.getHours();
                        if (hour < 23) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), hour + 1, m_time.getMinutes(), m_time.getSeconds()));
                            RefereshHourPanel();
                        }
                    }
                }
            }]);

            //fill hour table
            var items = [];
            for (var i = 0; i < 24; i++) {
                items.push({
                    text: i + String.Empty,
                    tag: i,
                    styles: {
                        backgroundColor: i == hour ? m_hourSelectedBackgroundColor : String.Empty,
                        color: m_hourTextColor,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_monthHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTime(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), this.tag, m_time.getMinutes(), m_time.getSeconds()));
                        }
                    }
                });
            }
            FillTable(m_hourTable, {
                fontSize: m_hourFontSize + "px"
            }, items);
        }

        /*
        * Create the panel used to show minutes.
        */
        function CreateMinutePanel() {
            m_minutePanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_minutePanel.appendChild(m_minuteHeader = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            m_minutePanel.appendChild(m_minuteTable = CreateTable(6, 10, m_tableWidth, m_minuteRowHeight));
        }

        /*
        * Referesh minute panel.
        */
        function RefereshMinutePanel() {
            var minute = m_time.getMinutes();

            //fill minute header
            FillTable(m_minuteHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Previous minute",
                    onclick: function () {
                        var minute = m_time.getMinutes();
                        if (minute > 0) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), minute - 1, m_time.getSeconds()));
                            RefereshMinutePanel();
                        }
                    }
                }
            }, {
                text: minute + " m",
                tag: minute,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose date",
                    onclick: function () {
                        RefereshDayPanel();
                        ShowPanel(m_dayPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Next minute",
                    onclick: function () {
                        var minute = m_time.getMinutes();
                        if (minute < 59) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), minute + 1, m_time.getSeconds()));
                            RefereshMinutePanel();
                        }
                    }
                }
            }]);

            //fill minute table
            var items = [];
            for (var i = 0; i < 60; i++) {
                items.push({
                    text: i + String.Empty,
                    tag: i,
                    styles: {
                        backgroundColor: i == minute ? m_minuteSelectedBackgroundColor : String.Empty,
                        color: m_minuteTextColor,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_monthHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTime(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), this.tag, m_time.getSeconds()));
                        }
                    }
                });
            }
            FillTable(m_minuteTable, {
                fontSize: m_minuteFontSize + "px"
            }, items);
        }

        /*
        * Create the panel used to show seconds.
        */
        function CreateSecondPanel() {
            m_secondPanel = DocumentUtility.CreateElement("div", null, {
                width: m_tableWidth + "px",
                height: "auto",
                margin: m_margin + "px",
                padding: "0px",
                overflow: "hidden"
            });
            m_secondPanel.appendChild(m_secondHeader = CreateTable(1, 3, m_tableWidth, m_headerHeight));
            m_secondPanel.appendChild(m_secondTable = CreateTable(6, 10, m_tableWidth, m_secondRowHeight));
        }

        /*
        * Referesh second panel.
        */
        function RefereshSecondPanel() {
            var second = m_time.getSeconds();

            //fill second header
            FillTable(m_secondHeader, {
                backgroundColor: m_headerBackgroundColor,
                fontSize: m_headerFontSize + "px"
            }, [{
                text: "＜",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Previous second",
                    onclick: function () {
                        var second = m_time.getSeconds();
                        if (second > 0) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), m_time.getMinutes(), second - 1));
                            RefereshSecondPanel();
                        }
                    }
                }
            }, {
                text: second + " s",
                tag: second,
                styles: {
                    cursor: "pointer"
                },
                attributes: {
                    title: "Click to choose date",
                    onclick: function () {
                        RefereshDayPanel();
                        ShowPanel(m_dayPanel);
                    }
                }
            }, {
                text: "＞",
                styles: {
                    width: m_arrowWidth + "px",
                    borderStyle: "solid",
                    borderWidth: "1px",
                    borderColor: m_headerButtonBorderColor,
                    backgroundColor: m_headerButtonBackgroundColor,
                    color: m_headerButtonTextColor,
                    fontWeight: "bold",
                    cursor: "pointer"
                },
                attributes: {
                    title: "Next second",
                    onclick: function () {
                        var second = m_time.getSeconds();
                        if (second < 59) {
                            SetTimeValue(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), m_time.getMinutes(), second + 1));
                            RefereshSecondPanel();
                        }
                    }
                }
            }]);

            //fill second table
            var items = [];
            for (var i = 0; i < 60; i++) {
                items.push({
                    text: i + String.Empty,
                    tag: i,
                    styles: {
                        backgroundColor: i == second ? m_secondSelectedBackgroundColor : String.Empty,
                        color: m_secondTextColor,
                        cursor: "default"
                    },
                    attributes: {
                        onmouseover: function () {
                            this._outColor = this.style.color;
                            this.style.color = m_monthHoverTextColor;
                        },
                        onmouseout: function () {
                            this._outColor && (this.style.color = this._outColor);
                        },
                        onclick: function () {
                            SetTime(new Date(m_time.getFullYear(), m_time.getMonth(), m_time.getDate(), m_time.getHours(), m_time.getMinutes(), this.tag));
                        }
                    }
                });
            }
            FillTable(m_secondTable, {
                fontSize: m_secondFontSize + "px"
            }, items);
        }

        /*
        * Show the datetime picker.
        */
        this.Show = Show;

        /*
        * Hide the datetime picker.
        */
        this.Close = Close;

        /*
        * Hide the datetime picker and accept the display time.
        */
        this.Ensure = Ensure;

        /*
        * Gets current display time.
        */
        this.GetTime = function () {
            return m_hasTime ? new Date(m_time.getTime()) : null;
        };

        /*
        * Set current display time.
        */
        this.SetTime = SetTime;

        /*
        * Clear current display time.
        */
        this.ClearTime = ClearTime;

        /*
        * Gets the TimeChanged event.
        */
        this.GetTimeChangedEvent = function () {
            return m_timeChanged;
        };
    };

    /***************** Paging Class Definition *****************/

    /*
    * Class Description:
    * Represents a paging control.
    *
    * Parameters:
    * parameters: The parameters for creating this object which is a object as follow:
    * {
    *   Container: The container of all paging elements.
    *   Maximum: The maximum number of displayed page-number anchors.
    *   IsRightToLeft: If this value is true, then creating the last page number anchor first.
    * }
    */
    var Paging = Xphter.Paging = function (parameters) {
        var m_container;
        var m_maximum = 10;
        var m_isRightToLeft = false;

        var m_create = new Event(this, "Create");
        var m_click = new Event(this, "Click");

        Constructor();

        /*
        * Constructor.
        */
        function Constructor() {
            if (!DataTypeIdentity.IsNotNullObject(parameters)) {
                throw new Error("The parameters to initializing Paging class is undefined.");
            }
            if (!parameters.Container) {
                throw new Error("The container of paging class is undefined.");
            }

            //analyze parameters
            m_container = parameters.Container;
            !DataTypeIdentity.IsUndefined(parameters.IsRightToLeft) && (m_isRightToLeft = !!parameters.IsRightToLeft);
            !global.isNaN(parameters.Maximum) && (m_maximum = Math.max(1, Math.floor(parameters.Maximum - 0)));

            //initialize
            DocumentUtility.ClearChildren(m_container);
        }

        /*
        * Method Description:
        * Create anchor of the specified page-number.
        * 
        * Parameters:
        * pageNumber: The page number.
        */
        function CreateAnchor(type, pageNumber) {
            var text = pageNumber + String.Empty;
            switch (type) {
                case Paging.PageNumberType.First:
                    text = "First";
                    break;
                case Paging.PageNumberType.Last:
                    text = "Last";
                    break;
                case Paging.PageNumberType.Previous:
                    text = "Previous";
                    break;
                case Paging.PageNumberType.Next:
                    text = "Next";
                    break;
                case Paging.PageNumberType.Ellipsis:
                    text = "...";
                    break;
            }
            var isAnchor = (type != Paging.PageNumberType.Current) && (type != Paging.PageNumberType.Ellipsis);

            var anchor = DocumentUtility.CreateElement("a", {
                target: "_blank"
            }, {
                display: "inline-block",
                cursor: isAnchor ? "pointer" : "default",
                textDecoration: isAnchor ? String.Empty : "none"
            }, text);

            m_create.Raise({
                Type: type,
                PageNumber: pageNumber,
                Anchor: anchor
            });

            EventUtility.Register(anchor, "click", function (e) {
                if (!isAnchor) {
                    e.preventDefault();
                    return;
                }

                var args = {
                    Cancel: false,
                    PageNumber: pageNumber
                };
                m_click.Raise(args);

                if (args.Cancel) {
                    e.preventDefault();
                }
            }, false);

            m_container.appendChild(anchor);
        }

        /*
        * Method Description:
        * Refresh the paging control use the specified data.
        * 
        * Parameters:
        * pageCount: The total count of pages.
        * pageNumber: The index of current page which start from one.
        */
        this.Refresh = function (pageCount, pageNumber) {
            var _pageCount = Math.floor(pageCount - 0);
            var _pageNumber = Math.floor(pageNumber - 0);
            if (_pageCount > 0 && _pageNumber < 1 && _pageNumber > _pageCount) {
                throw new Error("The page number is less than one or greater than number of pages.");
            }

            this.Clear();
            if (_pageCount < 2) {
                return;
            }

            var anchor = null;
            var start = (Math.ceil(_pageNumber / m_maximum) - 1) * m_maximum + 1;
            var end = Math.min(_pageCount, start + m_maximum - 1);

            if (m_isRightToLeft) {
                if (_pageNumber < _pageCount) {
                    CreateAnchor(Paging.PageNumberType.Last, _pageCount);
                    CreateAnchor(Paging.PageNumberType.Next, _pageNumber + 1);
                }
                if (end < _pageCount) {
                    CreateAnchor(Paging.PageNumberType.Other, _pageCount);
                    CreateAnchor(Paging.PageNumberType.Ellipsis, 0);
                }
                for (var i = end; i >= start; i--) {
                    CreateAnchor(i == _pageNumber ? Paging.PageNumberType.Current : Paging.PageNumberType.Other, i);
                }
                if (_pageNumber > 1) {
                    CreateAnchor(Paging.PageNumberType.Previous, _pageNumber - 1);
                    CreateAnchor(Paging.PageNumberType.First, 1);
                }
            } else {
                if (_pageNumber > 1) {
                    CreateAnchor(Paging.PageNumberType.First, 1);
                    CreateAnchor(Paging.PageNumberType.Previous, _pageNumber - 1);
                }
                for (var i = start; i <= end; i++) {
                    CreateAnchor(i == _pageNumber ? Paging.PageNumberType.Current : Paging.PageNumberType.Other, i);
                }
                if (end < _pageCount) {
                    CreateAnchor(Paging.PageNumberType.Ellipsis, 0);
                    CreateAnchor(Paging.PageNumberType.Other, _pageCount);
                }
                if (_pageNumber < _pageCount) {
                    CreateAnchor(Paging.PageNumberType.Next, _pageNumber + 1);
                    CreateAnchor(Paging.PageNumberType.Last, _pageCount);
                }
            }
        };

        /*
        * Method Description:
        * Clear all page numbers.
        */
        this.Clear = function () {
            DocumentUtility.ClearChildren(m_container);
        };

        /*
        * Method Description:
        * Gets the Create event which occurs after creating a anchor.
        *
        * The argument will pass to event handlers as follows:
        * {
        *   Type: The type of created page-number.
        *   PageNumber: The page number which start from one.
        *   Anchor: The created anchor element.
        * }
        */
        this.GetCreateEvent = function () {
            return m_create;
        };

        /*
        * Method Description:
        * Gets the Click event which occurs when user click a page-number anchor.
        *
        * The argument will pass to event handlers as follows:
        * {
        *   Cancel: Indicate whether cancel the default behavior of this anchor element.
        *   PageNumber: The page number which start from one.
        * }
        */
        this.GetClickEvent = function () {
            return m_click;
        };
    };

    /*
    * The type of page-number anchor.
    */
    Paging.PageNumberType = {
        /*
        * Current page.
        */
        Current: 0,

        /*
        * First page.
        */
        First: 1,

        /*
        * Last page.
        */
        Last: 2,

        /*
        * Previous page.
        */
        Previous: 3,

        /*
        * Next page.
        */
        Next: 4,

        /*
        * Other page.
        */
        Other: 5,

        /*
        * The ellipsis anchor.
        */
        Ellipsis: 6
    };

    /***************** AnchorsRoll Class Definition *****************/

    /*
    * Class Description:
    * Represents a roll control with some anchors.
    *
    * Parameters:
    * parameters: The parameters for creating this object which is a object as follow:
    * {
    *   Anchors: The array of anchors and targets.   
    *   TopPosition: The top position to place the target when user click a anchor.
    *   TopLine: When the Y position of a target is less than or equal this value, the anchor will be selected.
    *   BottomLine: When the Y position of a target is greater than or equal this value, the anchor will be unselected.
    * }
    */
    var AnchorsRoll = Xphter.AnchorsRoll = function (parameters) {
        var m_anchors = [];
        var m_topPosition = 100;
        var m_topLine = 0.5;
        var m_bottomLine = 0.5;

        var m_animator = null;
        var m_selectedIndex = -1;

        //Event: SelectedAnchorChanged
        var m_selectedAnchorChanged = new Event(this, "SelectedAnchorChanged");

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            parameters.Anchors && parameters.Anchors.ForEach(function (item, index) {
                if (!item) {
                    return;
                }

                item.Anchor._anchorIndex = m_anchors.length;
                m_anchors.push(item);
            });
            !global.isNaN(parameters.TopPosition) && (m_topPosition = parameters.TopPosition);
            !global.isNaN(parameters.TopLine) && (m_topLine = Math.max(0, parameters.TopLine));
            !global.isNaN(parameters.BottomLine) && (m_bottomLine = Math.max(0, parameters.BottomLine));

            var viewportHeight = BrowserGeometry.GetViewportHeight();
            m_topLine <= 1 && (m_topLine = viewportHeight * m_topLine);
            m_bottomLine <= 1 && (m_bottomLine = viewportHeight * m_bottomLine);
            m_topLine = Math.min(m_topLine, m_bottomLine);
            m_bottomLine = Math.max(m_topLine, m_bottomLine);

            m_anchors.ForEach(function (item) {
                EventUtility.Register(item.Anchor, "click", function (event) {
                    return ScrollTo(event.currentTarget._anchorIndex);
                }, false);
            });
            EventUtility.Register(global, "scroll", OnScroll, false);
        }

        /*
        * Private Method Description:
        * Scroll window to show the target in the specified position.
        * 
        * Parameters:
        * index: The target index.
        */
        function ScrollTo(index) {
            var target = m_anchors[index].Target;
            var scrollY = BrowserGeometry.GetVerticalScroll();
            var offset = m_topPosition - (target.offsetTop - scrollY);
            var duration = 1000;

            m_animator && m_animator.Stop();
            m_animator = new Animator({
                Fps: 30,
                Duration: duration,
                OnFrame: function (elapsedTime) {
                    global.scrollTo(0, Math.max(0, scrollY - (offset / duration * elapsedTime)));
                },
                OnDispose: function () {
                    global.scrollTo(0, Math.max(0, scrollY - offset));
                    Select(index);
                }
            });
            m_animator.Start();
        }

        /*
        * Private Method Description:
        * Select a anchor in the specified position.
        * 
        * Parameters:
        * index: The anchor index.
        */
        function Select(index) {
            if (index > m_anchors.length) {
                throw new Error("index is greater than total anchor number.");
            }
            if ((index = Math.max(-1, index)) == m_selectedIndex) {
                return;
            }

            m_anchors.ForEach(function (item) {
                item.IsSelected = false;
            });
            if (index >= 0) {
                m_anchors[index].IsSelected = true;
            }

            var unselectedIndex = m_selectedIndex;
            m_selectedIndex = index;
            m_selectedAnchorChanged.Raise({
                SelectedIndex: index,
                UnselectedIndex: unselectedIndex,
                SelectedAnchor: index >= 0 ? m_anchors[index].Anchor : null,
                UnselectedAnchor: unselectedIndex >= 0 && unselectedIndex < m_anchors.length ? m_anchors[unselectedIndex].Anchor : null
            });
        }

        /*
        * Private Method Description:
        * The event handler for window.onscroll.
        */
        function OnScroll() {
            var y = 0;
            var viewportHeight = Xphter.BrowserGeometry.GetViewportHeight();
            var scrollY = Xphter.BrowserGeometry.GetVerticalScroll();
            for (var i = m_anchors.length - 1; i >= 0; i--) {
                y = m_anchors[i].Target.offsetTop - scrollY;
                if (y < 0 || y > viewportHeight) {
                    continue;
                }

                if (viewportHeight - y <= m_bottomLine) {
                    if (i) {
                        Select(i - 1);
                    } else {
                        Select(i);
                    }
                    break;
                }
                if (y <= m_topLine) {
                    Select(i);
                    break;
                }
            }
        }

        /*
        * Method Description:
        * Selected the default anchor.
        */
        function SelectDefaultAnchor() {
            var selectedIndex = -1;
            for (var i = 0; i < m_anchors.length; i++) {
                if (m_anchors[i].IsSelected) {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex >= 0) {
                Select(selectedIndex);
            } else if (m_anchors.length > 0) {
                Select(0);
            }
        }

        /*
        * Method Description:
        * Selected the default anchor.
        */
        this.SelectDefault = SelectDefaultAnchor;

        /*
        * Method Description:
        * Gets SelectedAnchorChanged event.
        */
        this.GetSelectedAnchorChangedEvent = function () {
            return m_selectedAnchorChanged;
        };
    };

    /***************** LazyImages Class Definition *****************/

    /*
    * Class Description:
    * Provides lazy load function of images in the target container.
    *
    * Parameters:
    * parameters: The parameters for creating this object which is a object as follow:
    * {
    *   Container: The container which contains the images to load lazy.
    *   Threshold: The distance from top of viewport or bottom of viewport.
    *   LoadDelay: The number of milliseconds delay to load image when it appear in browser viewport.
    * }
    */
    var LazyImages = Xphter.LazyImages = function (parameters) {
        var m_container = null;
        var m_images = [];
        var m_threshold = 0;
        var m_loadDelay = 100;
        var m_timer = 0;

        var m_isLoading = false;
        var m_loadingQueue = [];

        var m_processedAttributeName = "processed";
        var m_loadedAttributeName = "loaded";

        var m_sourceAttributeName = "source";
        var m_lazyAttributeName = "lazy";
        var m_loadingAttributeName = "loading";

        Constructor();

        /*
        * Consturctor.
        */
        function Constructor() {
            // read parameters
            if (DataTypeIdentity.IsNotNullObject(parameters)) {
                if (DataTypeIdentity.IsNotNullObject(parameters.Container)) {
                    m_container = parameters.Container;
                }

                !global.isNaN(parameters.Threshold) && (m_threshold = Math.max(0, parameters.Threshold - 0));
                !global.isNaN(parameters.LoadDelay) && (m_loadDelay = Math.max(0, parameters.LoadDelay - 0));
            }

            // initialize images
            InitializeImages();

            // register events
            EventUtility.Register(global, "resize", StartCheckTimer, false);
            EventUtility.Register(global, "scroll", StartCheckTimer, false);

            // perform once first
            global.setTimeout(CheckImages, 0);
        }

        /*
        * Method Description:
        * Initializes all lazy images.
        * Note: IE before vesion 8 not support element.hasAttribute method.
        */
        function InitializeImages() {
            var y = 0;
            var src = null;
            var image = null;
            var images = null;
            var processed = false;
            var viewportHeight = BrowserGeometry.GetViewportHeight();

            if (m_container) {
                images = m_container.getElementsByTagName("img");
            } else {
                images = global.document.images;
            }

            m_images.length = 0;
            for (var i = 0; i < images.length; i++) {
                image = images[i];

                // ignore loaded images
                if (image.complete) {
                    continue;
                }

                // ignore processed images
                if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 8) {
                    if (!image.getAttributeNode(m_lazyAttributeName) || image.getAttributeNode(m_loadedAttributeName)) {
                        continue;
                    }
                } else {
                    if (!image.hasAttribute(m_lazyAttributeName) || image.hasAttribute(m_loadedAttributeName)) {
                        continue;
                    }
                }

                // ignore appeared images
                y = DocumentUtility.GetElementY(image);
                if (!(y - m_threshold > viewportHeight || y + image.offsetHeight + m_threshold < 0)) {
                    image.setAttribute(m_loadedAttributeName, "true");
                    continue;
                }

                if (BrowserCapability.IsIE && BrowserCapability.IECompatibilityMode < 8) {
                    processed = image.getAttributeNode(m_processedAttributeName);
                } else {
                    processed = image.hasAttribute(m_processedAttributeName);
                }

                if (!processed) {
                    image.setAttribute(m_sourceAttributeName, image.src);
                    image.src = String.Empty;/* must set empty to cancel download image  */
                    image.src = image.getAttribute(m_loadingAttributeName);

                    image.setAttribute(m_processedAttributeName, "true");
                }

                m_images.push(image);
            }
        }

        /*
        * Method Description:
        * Start the timer to check images.
        */
        function StartCheckTimer() {
            if (m_timer) {
                global.clearTimeout(m_timer);
                m_timer = 0;
            }

            m_timer = global.setTimeout(CheckImages, m_loadDelay);
        }

        /*
        * Method Description:
        * Checks all images appear in the browser viewport.
        */
        function CheckImages() {
            var y = 0;
            var src = null;
            var image = null;
            var newImage = null;
            var loadingImage = null;
            var viewportHeight = BrowserGeometry.GetViewportHeight();

            for (var i = 0; i < m_images.length; i++) {
                if (!(image = m_images[i])) {
                    continue;
                }

                // ignore not appeared images
                y = DocumentUtility.GetElementY(image);
                if (y - m_threshold > viewportHeight || y + image.offsetHeight + m_threshold < 0) {
                    continue;
                }

                src = image.getAttribute(m_sourceAttributeName);
                loadingImage = m_loadingQueue.Where(function (item) {
                    return item.Url == src;
                }).FirstOrNull();
                if (loadingImage != null) {
                    if (loadingImage.Loaded) {
                        image.src = src;
                    } else {
                        loadingImage.Targets.push(image);
                    }
                } else {
                    m_loadingQueue.push({
                        Url: src,
                        Targets: [image],
                        Loaded: false
                    });
                }
                image.setAttribute(m_loadedAttributeName, "true");

                m_images[i] = null;
            }

            StartLoadImages();
        }

        /*
        * Method Description:
        * Start the operation to load images.
        */
        function StartLoadImages() {
            if (m_isLoading) {
                return;
            }

            LoadImage();
        }

        /*
        * Method Description:
        * Start the operation to load images.
        */
        function LoadImage() {
            if (m_loadingQueue.length == 0) {
                m_isLoading = false;
                return;
            }
            m_isLoading = true;

            var loadingImage = m_loadingQueue[0];
            var newImage = new Image();
            newImage.onload = OnLoadImage;
            newImage.onerror = OnLoadImage;
            newImage.src = loadingImage.Url;
        }

        /*
        * Method Description:
        * Invokes when load image completed.
        */
        function OnLoadImage() {
            var src = this.src;
            var loadingImage = m_loadingQueue[0];

            loadingImage.Loaded = true;

            var targets = loadingImage.Targets;
            for (var i = 0; i < targets.length; i++) {
                targets[i].src = src;
            }

            m_loadingQueue.shift();
            LoadImage();
        }

        /*
        * Method Description:
        * Initialize all lazy images. Client should call this method when HTML DOM has changed.
        */
        this.Initialize = InitializeImages;
    };
})(window);