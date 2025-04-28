function spCreateActiviyUpdateUserProfile(activityDoc, docType) {

    if (!activityDoc) {
        throw new Error("Parameter 'activityDoc' is null or undefined.");
    }

    var collections = getContext().getCollection();


    var filterQuery =
    {
        'query': 'SELECT * FROM c where c.userId = @userId AND c.activityType = @docType',
        'parameters': [
            { 'name': '@userId', 'value': activityDoc.userId },
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
                getContext().getResponse.setBody("Query Returned No Results");
            }
            else {

                var metadataItem = result[0];
                
                getContext().getResponse().setBody(JSON.stringify(metadataItem));
                if (metadataItem.reputation != null) {

                    if (activityDoc.segment in metadataItem.reputation) {
                        var value = JSON.stringify(metadataItem.reputation[activityDoc.segment]);

                        var obj = JSON.parse(value);

                        obj.pointValue = obj.pointValue + activityDoc.pointValue;
                        console.log(obj.pointValue);

                        if (metadataItem.reputation.ActivityStats != null && activityDoc.ActivityType in metadataItem.reputation.ActivityStats) {

                            var value = JSON.stringify(metadataItem.reputation.ActivityStats[activityDoc.ActivityType]);
                            value.PointValue = value.PointValue + activityDoc.pointValue;
                            value.CountValue += 1;
                        }
                        else {
                            var activityStats = new ActivityStats { ActivityType: activityDoc.ActivityType, PointValue: activityDoc.pointValue, CountValue: 1 };
                            metadataItem.reputation.ActivityStats[activityDoc.ActivityType] = activityStats;
                        }


                        metadataItem.reputation[activityDoc.segment] = obj;
                        getContext().getResponse().setBody(JSON.stringify(metadataItem));

                        var ac = new AccessCondition(Condition = metadataItem._etag, Type = AccessConditionType.IfMatch);

                        collections.replaceDocument(metadataItem._self, metadataItem, new RequestOptions { AccessCondition = ac }, function (err) {
                             if (err) throw err;
     
                             collections.createDocument(collections.getSelfLink(), activityDoc, function (err, doc) {
     
                                 if (err) throw err;
     
                                 getContext().getResponse().setBody(JSON.stringify(activityDoc));
     
                             });
                         });
                    }
                }
            }
        });
}