import { isNullOrWhiteSpace } from "../.fable/fable-library.3.1.11/String.js";

export function Config_variableOrDefault(key, defaultValue) {
    const foundValue = process.env[key] ? process.env[key] : '';
    if (isNullOrWhiteSpace(foundValue)) {
        return defaultValue;
    }
    else {
        return foundValue;
    }
}

