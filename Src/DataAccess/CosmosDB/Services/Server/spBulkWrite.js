//[version=7]
function spBulkWrite(activityDoc, userPofileDoc, docType) {

    if (!activityDoc) {
        throw new Error("Parameter 'activityDoc' is null or undefined.");
    }

    if (!userPofileDoc) {
        throw new Error("Parameter 'userPofileDoc' is null or undefined.");
    }

    var collections = getContext().getCollection();
    var count = 0;

    var response = { statusCode: 200, ErrorMessage: "", Count: 0 };

    var docsLength = activityDoc.length;
    if (docsLength == 0) {

        response.statusCode = 400,
            response.ErrorMessage = "Input document array is empty.";
        getContext().getResponse().setBody(response);
    }

    tryRetrieveDoc(activityDoc[count], userPofileDoc, docType, callback, recursiveCallback);

    function tryRetrieveDoc(activityDoc, userPofileDoc, docType, callback, recursiveCallback) {

        var filterQuery =
        {
            'query': 'SELECT * FROM c where c.userId = @userId AND c.docType = @docType',
            'parameters': [
                { 'name': '@userId', 'value': userPofileDoc.userId },
                { 'name': '@docType', 'value': docType },
            ]
        }

        var isAccepted = collections.queryDocuments(
            collections.getSelfLink(),
            filterQuery,
            {},
            function (err, result, options) {

                if (err) {
                    throw err;
                }

                if (!result || !result.length) {
                    tryCreate(userPofileDoc, activityDoc, callback, recursiveCallback);
                }
                else {
                    tryReplace(result, userPofileDoc, activityDoc, callback, recursiveCallback);
                }
            });

        if (!isAccepted) {
            response.statusCode = 300;
            response.msg = "Store procedure has been runing too long and get canceled by the server.";
            response.Count = count;
            getContext().getResponse().setBody(response);
        };
    }

    function tryCreate(userPofileDoc, activityDoc, callback, recursiveCallback) {
        var isAccepted = collections.createDocument(collections.getSelfLink(), userPofileDoc, callback);
        if (!isAccepted) throw new Error("Unable to schedule create userPofileDoc document");
        tryCreateActivityDoc(activityDoc, recursiveCallback);
        getContext().getResponse().setBody(JSON.stringify(activityDoc));
    }

    function tryReplace(docToReplace, userPofileDoc, activityDoc, callback) {
        var isAccepted = collections.replaceDocument(docToReplace[0]._self, userPofileDoc, callback);
        if (!isAccepted) throw new Error("Unable to schedule replace userPofileDoc document");
        tryCreateActivityDoc(activityDoc, recursiveCallback);
        getContext().getResponse().setBody(JSON.stringify(activityDoc));
    }

    function tryCreateActivityDoc(activityDoc, recursiveCallback) {
        var isAccepted = collections.createDocument(collections.getSelfLink(), activityDoc, recursiveCallback);
        if (!isAccepted) throw new Error("Unable to schedule create activityDoc document");
    }

    function callback(err, doc, options) {
        if (err) throw err;

    }

    function recursiveCallback(err, doc) {
        if (err) {
            throw err;
        }
        count++;

        if (count >= docsLength) {
            response.statusCode = 200;
            response.msg = "All documents inserted";
            response.Count = count;
            getContext().getResponse().setBody(response);
        } else {
            tryCreateActivityDoc(activityDoc[count], recursiveCallback);
        }
    }
}