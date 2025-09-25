@Structured @StructuredMedications @1.5.0-Full-Pack
Feature: StructuredMedications

# These Tests are only Testing this Structured Area in isolation and Not with other Areas or Combinations of Include Parameters
# Tests around Multiple Structured Areas in one Request are tested in the MultipleRequests Feature
# These are a minimal subset of tests to support the gp-connect-mock-tests A3

Scenario Outline: Retrieve the medication structured record section for a patient with no problems and including prescription issues
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "<Patient>"
		And I add the medication parameter with includePrescriptionIssues set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		# And the Bundle should be valid for patient "<Patient>"
		And check that the bundle does not contain any duplicate resources
		And the Bundle should contain "1" lists
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		And the List of MedicationStatements should be valid
		And there should only be one order request for acute prescriptions
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid
		And check the response does not contain an operation outcome
		And I Check There is No Problems Secondary Problems List
		And I Check No Problem Resources are Included

	Examples:
		| Patient   |
		| patient3  |
		| patient5  |
		| patient12 |
		| patient16 |

Scenario Outline: Retrieve the medication structured record for a patient with no problems and excluding prescription issues
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "<Patient>"
		And I add the medication parameter with includePrescriptionIssues set to "false"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		# And the Bundle should be valid for patient "<Patient>"
		And check that the bundle does not contain any duplicate resources
		And the Bundle should contain "1" lists
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		And the List of MedicationStatements should be valid
		And the Medication Requests should not contain any issues
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid
		And check the response does not contain an operation outcome
		And I Check There is No Problems Secondary Problems List
		And I Check No Problem Resources are Included
	Examples:
		| Patient  |
		| patient3 |
		| patient5 |
		| patient12 |

Scenario: Retrieve the medication structured record section for a patient with no medications
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient4"
		And I add the medication parameter with includePrescriptionIssues set to "false"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		# And the Bundle should be valid for patient "patient4"
		And the Bundle should contain "1" lists
		And the List of MedicationStatements should be valid
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		
Scenario: Retrieve the medication structured record section for a patient with a timePeriod
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient2"
		And I add the medications parameter to search from "3" years back
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		# And the Bundle should be valid for patient "patient2"		
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		And the List of MedicationStatements should be valid
		And the MedicationStatement EffectiveDate is Greater Than Search Date of "3" years ago	

Scenario: Retrieve the medication structured record section for a patient with medication prescribed elsewhere
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient12"
		And I add the medication parameter with includePrescriptionIssues set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		# And the Bundle should be valid for patient "patient12"
		And the Bundle should contain "1" lists
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		And the List of MedicationStatements should be valid
		And the MedicationStatement for prescriptions prescribed elsewhere should be valid
