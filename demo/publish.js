var ghPages = require("gh-pages");


// Update with wrapper repo
var packageUrl = "https://github.com/CompositionalIT/feliz-ag-grid.git";

console.log("Publishing to ", packageUrl);

ghPages.publish("dist", {
    repo: packageUrl
}, function (e) {
    if (e === undefined) {
        console.log("Finished publishing succesfully");
    } else {
        console.log("Error occured while publishing :(", e);
    }
});