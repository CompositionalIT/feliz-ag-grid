import { Mocha_runTests, Test_testCase, Test_testList } from "./.fable/Fable.Mocha.2.9.1/Mocha.fs.js";
import { int32ToString, structuralHash, assertEqual } from "./.fable/fable-library.3.1.11/Util.js";
import { singleton, ofArray, contains } from "./.fable/fable-library.3.1.11/List.js";
import { equals, class_type, string_type, float64_type, bool_type, int32_type } from "./.fable/fable-library.3.1.11/Reflection.js";
import { printf, toText } from "./.fable/fable-library.3.1.11/String.js";

export function add(x, y) {
    return x + y;
}

export const appTests = Test_testList("App tests", singleton(Test_testCase("add works", () => {
    let copyOfStruct;
    const actual = add(2, 3) | 0;
    const expected = 5;
    const msg = "Result must be 5";
    if ((actual === expected) ? true : (!(new Function("try {return this===window;}catch(e){ return false;}"))())) {
        assertEqual(actual, expected, msg);
    }
    else {
        let errorMsg;
        if (contains((copyOfStruct = (actual | 0), int32_type), ofArray([int32_type, bool_type, float64_type, string_type, class_type("System.Decimal"), class_type("System.Guid")]), {
            Equals: (x, y) => equals(x, y),
            GetHashCode: (x) => structuralHash(x),
        })) {
            const arg20 = int32ToString(actual);
            const arg10 = int32ToString(expected);
            errorMsg = toText(printf("\u003cspan style=\u0027color:black\u0027\u003eExpected:\u003c/span\u003e \u003cbr /\u003e\u003cdiv style=\u0027margin-left:20px; color:crimson\u0027\u003e%s\u003c/div\u003e\u003cbr /\u003e\u003cspan style=\u0027color:black\u0027\u003eActual:\u003c/span\u003e \u003c/br \u003e\u003cdiv style=\u0027margin-left:20px;color:crimson\u0027\u003e%s\u003c/div\u003e\u003cbr /\u003e\u003cspan style=\u0027color:black\u0027\u003eMessage:\u003c/span\u003e \u003c/br \u003e\u003cdiv style=\u0027margin-left:20px; color:crimson\u0027\u003e%s\u003c/div\u003e"))(arg10)(arg20)(msg);
        }
        else {
            errorMsg = toText(printf("\u003cspan style=\u0027color:black\u0027\u003eExpected:\u003c/span\u003e \u003cbr /\u003e\u003cdiv style=\u0027margin-left:20px; color:crimson\u0027\u003e%A\u003c/div\u003e\u003cbr /\u003e\u003cspan style=\u0027color:black\u0027\u003eActual:\u003c/span\u003e \u003c/br \u003e\u003cdiv style=\u0027margin-left:20px;color:crimson\u0027\u003e%A\u003c/div\u003e\u003cbr /\u003e\u003cspan style=\u0027color:black\u0027\u003eMessage:\u003c/span\u003e \u003c/br \u003e\u003cdiv style=\u0027margin-left:20px; color:crimson\u0027\u003e%s\u003c/div\u003e"))(expected)(actual)(msg);
        }
        throw (new Error(errorMsg));
    }
})));

export const allTests = Test_testList("All", singleton(appTests));

(function (args) {
    return Mocha_runTests(allTests);
})(typeof process === 'object' ? process.argv.slice(2) : []);

