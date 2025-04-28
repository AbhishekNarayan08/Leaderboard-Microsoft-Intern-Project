//[version=2]
function spUpsertUserProfileAndCreateActiviy(activityDoc, userPofileDoc, docType) {

    var collection = getContext().getCollection();
    getContext().getResponse().setBody(JSON.stringify(activityDoc));

    if (!activityDoc || !userPofileDoc)
        throw new Error("The document is undefined or null.");

    retrieveDoc(activityDoc, userPofileDoc, docType, callback);

    function retrieveDoc(activityDoc, userPofileDoc, docType, callback) {

        var filterQuery =
        {
            'query': 'SELECT * FROM c where c.userId = @userId AND c.docType = @docType',
            'parameters': [
                { 'name': '@userId', 'value': userPofileDoc.userId },
                { 'name': '@docType', 'value': docType },
            ]
        }

        var isAccepted = collection.queryDocuments(
            collection.getSelfLink(),
            filterQuery, {},
            function (err, retrievedDocs) {
                if (err) throw err;
                getContext().getResponse().setBody(JSON.stringify(retrievedDocs));

                if (retrievedDocs.length > 0) {
                    console.log(retrievedDocs.length);
                    tryReplace(retrievedDocs, userPofileDoc, activityDoc, callback);
                } else {
                    tryCreate(userPofileDoc, activityDoc, callback);
                }
            });
        if (!isAccepted) throw new Error("Unable to query documents");
    }

    function tryCreate(userPofileDoc, activityDoc, callback) {
        var isAccepted = collection.createDocument(collection.getSelfLink(), userPofileDoc, callback);
        if (!isAccepted) throw new Error("Unable to schedule create userPofileDoc document");
        tryCreateActivityDoc(activityDoc, callback);
        getContext().getResponse().setBody(JSON.stringify(activityDoc));
    }

    function tryReplace(docToReplace, userPofileDoc, activityDoc, callback) {
        var isAccepted = collection.replaceDocument(docToReplace[0]._self, userPofileDoc, callback);
        if (!isAccepted) throw new Error("Unable to schedule replace userPofileDoc document");
        tryCreateActivityDoc(activityDoc, callback);
        getContext().getResponse().setBody(JSON.stringify(activityDoc));
    }

    function tryCreateActivityDoc(activityDoc, callback) {
        var isAccepted = collection.createDocument(collection.getSelfLink(), activityDoc, callback);
        if (!isAccepted) throw new Error("Unable to schedule create activityDoc document");
    }

    function callback(err, doc, options) {
        if (err) throw err;

    }
}