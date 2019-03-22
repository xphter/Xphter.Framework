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

})(window);