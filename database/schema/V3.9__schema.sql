DO $$
BEGIN
	IF NOT EXISTS (SELECT FROM configuration.sds_query WHERE query_name='GetGpConsumerAsIdByOdsCode') THEN
		INSERT INTO configuration.sds_query 
		(
			query_name, 
			search_base, 
			query_text, 
			query_attributes
		)
		VALUES 
		(
			'GetGpConsumerAsIdByOdsCode',
			'ou=services, o=nhs',
			'(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1))',
			'uniqueIdentifier'
		);
	END IF;
END
$$;