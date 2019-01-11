(function (global, undefined) {
    global.onerror = function (message, url, line) {
        global.top.ProcessFunctionResult(message);
        return true;
    };

    var Assert = global.Assert = {
        AreEqual: function (expected, actual, message) {
            if (expected != actual) {
                if (message === undefined) {
                    message = "The expected value is " + expected + ", but the actual value is " + actual + ".";
                }
                throw new Error(message);
            }
        },

        AreNotEqual: function (expected, actual, message) {
            if (expected == actual) {
                if (message === undefined) {
                    message = "The expected value is " + expected + " and the actual value is " + actual + " too.";
                }
                throw new Error(message);
            }
        },

        AreSame: function (expected, actual, message) {
            if (expected !== actual) {
                if (message === undefined) {
                    message = "The expected value is " + expected + ", but the actual value is " + actual + ".";
                }
                throw new Error(message);
            }
        },

        AreNotSame: function (expected, actual, message) {
            if (expected === actual) {
                if (message === undefined) {
                    message = "The expected value is " + expected + " and the actual value is " + actual + " too.";
                }
                throw new Error(message);
            }
        },

        IsTrue: function (condition, message) {
            if (!condition) {
                if (message === undefined) {
                    message = "The condition is not true.";
                }
                throw new Error(message);
            }
        },

        IsFalse: function (condition, message) {
            if (!!condition) {
                if (message === undefined) {
                    message = "The condition is not false.";
                }
                throw new Error(message);
            }
        },

        IsNull: function (actual, message) {
            if (actual !== null) {
                if (message === undefined) {
                    message = "The actual value is not null.";
                }
                throw new Error(message);
            }
        },

        IsNotNull: function (actual, message) {
            if (actual === null) {
                if (message === undefined) {
                    message = "The actual value is null.";
                }
                throw new Error(message);
            }
        }
    };
})(window);