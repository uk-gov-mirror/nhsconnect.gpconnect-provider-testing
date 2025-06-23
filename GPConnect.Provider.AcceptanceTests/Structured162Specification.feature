@Structured162Specification
Feature: Structured162Specification

#Update to MedicationRequest.medication and MedicationStatement.medication datatype
Scenario: Retrieve the medication structured record section for a patient whose record has single or multiple medications
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient37"
		And I add the medication parameter with includePrescriptionIssues set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the Medication MedicationRequest Medication type should be valid
		And the Medication MedicationStatement Medication type should be valid
	
#Update to dose syntax to support Information Standards Notice DAPB4013
Scenario: Retrieve the medication structured record section for patient with legacy medications
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient12"
		And I add the medication parameter with includePrescriptionIssues set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the MedicationRequest Legacy Dosage Instruction should be valid
		And the MedicationStatement Legacy Dosage should be valid

Scenario: Retrieve the medication structured record section for patients with structured medications
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient37"
		And I add the medication parameter with includePrescriptionIssues set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the MedicationRequest Structured Dosage Instruction should be valid
		And the MedicationStatement Structured Dosage should be valid

#Ensure OperationDefinition aligns with profile
Scenario: Retrieve medication structured record with invalid filterPrescriptionType partParameter
	Given I configure the default "GpcGetStructuredRecord" request
	And I add an NHS Number parameter for "patient39"
	And I add the medication parameter with filterPrescriptionType set to "repeat"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
	And Check the operation outcome returns the correct text and diagnostics includes "includeMedication" and "filterPrescriptionType"

Scenario: Retrieve problems structured record with invalid problem significance partParameter
	Given I configure the default "GpcGetStructuredRecord" request
	And I add an NHS Number parameter for "patient39"
	And I add the problems parameter with filterSignificance "minor"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
	And Check the operation outcome returns the correct text and diagnostics includes "includeProblems" and "filterSignificance"

#Specimen collection.extension[fastingStatus] to include coded concept
Scenario: Verify Investigations structured record for a Patient with Investigations not linked to any problems
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient38"
		And I add the Investigations parameter
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		And the Bundle should be valid for patient "patient38"
	    And check that the bundle does not contain any duplicate resources
		And check the response does not contain an operation outcome
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid 
		And the Bundle should contain "1" lists
		And I Check the Investigations List is Valid
		And The Structured List Does Not Include Not In Use Fields	
		And I Check the DiagnosticReports are Valid
		And I Check the DiagnosticReports Do Not Include Not in Use Fields
		And I Check the Specimens are Valid		
		And I Check the Specimens Do Not Include Not in Use Fields
		And I Check the Test report Filing is Valid
		And I Check the Test report Filing Do Not Include Not in Use Fields
		And I Check There is No Problems Secondary Problems List
		And I Check No Problem Resources are Included

#New requirement for resolved allergies to be returned as transfer-degraded drug allergies
Scenario Outline: Retrieve the allergy structured record section for a patient including resolved allergies no problems associated
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "<Patient>"
		And I add the allergies parameter with resolvedAllergies set to "true"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		And the AllergyIntolerance should be valid
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid
		And the Bundle should be valid for patient "<Patient>"
		And check that the bundle does not contain any duplicate resources
		And the Bundle should contain "2" lists
		And the Bundle should contain a list with the title "Allergies and adverse reactions"
		And the Bundle should contain a list with the title "Ended allergies"
		And the Bundle should contain the correct number of allergies
		And the Lists are valid for a patient with allergies
		And check the response does not contain an operation outcome
		And I Check There is No Problems Secondary Problems List
		And I Check No Problem Resources are Included
	Examples:
		| Patient   |
		| patient39 |

#Resources may contain a ‘no disclosure to patient’ security label
Scenario: Structured request sent for multiple clinical areas where each applicable resource may contain a no disclosure to patient security label
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient39"
		And I add the allergies parameter with resolvedAllergies set to "true"
		And I add the medication parameter with includePrescriptionIssues set to "true"
		And I add the includeConsultations parameter only
		And I add the Problems parameter
		And I add the immunizations parameter
		And I add the uncategorised data parameter
		And I add the Investigations parameter
		And I add the Referrals parameter
    When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And check that the bundle does not contain any duplicate resources
		And the patient resource in the bundle should contain meta data profile and version id
		And check the response does not contain an operation outcome	
		And check that each applicable resource may contain a no disclosure to patient security label

Scenario: Migrate Patient39 With Sensitive where each applicable resource may contain a no disclosure to patient security label
	Given I configure the default "MigrateStructuredRecordWithSensitive" request
	And I add an NHS Number parameter for "patient39"
		And I add the includeFullrecord parameter with includeSensitiveInformation set to "true"
    When I make the "MigrateStructuredRecordWithSensitive" request
	Then the response status code should indicate success
		And check that the bundle does not contain any duplicate resources
		And the patient resource in the bundle should contain meta data profile and version id
		And check the response does not contain an operation outcome
		And I Check Documents have been Returned and save the first documents url for retrieving later
		And I Check the returned DocumentReference is Valid
		And I Check the returned DocumentReference Do Not Include Not In Use Fields
		And check that each applicable resource may contain a no disclosure to patient security label

#Updates to capability statement
Scenario: Fhir Get Metadata and Check Version of Structured CapabilityStatement
	Given I configure the default "StructuredMetaDataRead" request
	When I make the "MetadataRead" request
	Then the response status code should indicate success
		And the Response Resource should be a CapabilityStatement
		And the Structured CapabilityStatement version should match the GP Connect specification release

Scenario: Fhir Get Metadata and Check Version of Foundations And Appointments CapabilityStatement
	Given I configure the default "MetadataRead" request
	When I make the "MetadataRead" request
	Then the response status code should indicate success
		And the Response Resource should be a CapabilityStatement
		And the FoundationsAndAppmts CapabilityStatement version should match the GP Connect specification release

Scenario: Fhir Get Metadata and Check Version of Documents CapabilityStatement
	Given I configure the default "DocumentsMetaDataRead" request
	When I make the "MetadataRead" request
	Then the response status code should indicate success
		And the Response Resource should be a CapabilityStatement
		And the Documents CapabilityStatement version should match the GP Connect specification release

Scenario Outline: Structured CapabilityStatement returns correct profile versions
Given I configure the default "StructuredMetaDataRead" request
	When I make the "StructuredMetaDataRead" request
	Then the response status code should indicate success
	And the CapabilityStatement REST Operations should contain "gpc.getstructuredrecord"
    And the CapabilityStatement Profile should contain the correct reference and version history "<urlToCheck>" 
Examples: 
| urlToCheck                                                                                          |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1/_history/1.8                 |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1/_history/1.4            |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1/_history/1.3            |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-PractitionerRole-1/_history/1.2        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-AllergyIntolerance-1/_history/1.7      |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Medication-1/_history/1.2              |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationStatement-1/_history/1.8     |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationRequest-1/_history/1.6       |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-List-1/_history/1.7                    |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-StructuredRecord-Bundle-1/_history/1.3       |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1/_history/1.2              |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Immunization-1/_history/1.5            |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ProblemHeader-Condition-1/_history/1.8 |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Encounter-1/_history/1.6.1             |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Observation-1/_history/1.7             |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-DiagnosticReport-1/_history/1.3        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Specimen-1/_history/1.3                |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ProcedureRequest-1/_history/1.4        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ReferralRequest-1/_history/1.2         |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-DocumentReference-1/_history/1.3       |
| https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-HealthcareService-1/_history/1.0       |

Scenario Outline: Documents CapabilityStatement returns correct profile versions
Given I configure the default "DocumentsMetaDataRead" request
	When I make the "DocumentsMetaDataRead" request
	Then the response status code should indicate success
    And the CapabilityStatement Profile should contain the correct reference and version history "<urlToCheck>" 
Examples: 
| urlToCheck                                                                                    |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1/_history/1.8           |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1/_history/1.4      |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1/_history/1.3      |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-PractitionerRole-1/_history/1.2  |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1/_history/1.2        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-DocumentReference-1/_history/1.3 |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Searchset-Bundle-1/_history/1.3        |

#####################Regression

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
		And the Bundle should be valid for patient "<Patient>"
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

Scenario Outline: Retrieve the medication structured record section for a patient with problems linked and including prescription issues
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
		And the Bundle should be valid for patient "<Patient>"
		And check that the bundle does not contain any duplicate resources
		And the Bundle should contain "2" lists
		And the Medications should be valid
		And the Medication Statements should be valid
		And the Medication Requests should be valid
		And the List of MedicationStatements should be valid
		And there should only be one order request for acute prescriptions
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid
		And check the response does not contain an operation outcome
		And I Check The Problems Secondary Problems List
		And I Check The Problems Secondary Problems List Does Not Include Not In Use Fields
		And I Check The Problems Resources are Valid
		And I check The Problem Resources Do Not Include Not In Use Fields
		And Check a Problem is Linked to a MedicationRequest resource that has been included in the response
		And Check the MedicationRequests have a link to a medication that has been included in response
		And Check there is a MedicationStatement resource that is linked to the MedicationRequest and Medication
		And Check the Medications List resource has been included in response
	Examples:
		| Patient   |
		| patient2  |

Scenario Outline: Retrieve problems structured record with status partParameter expected success
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient2"
		And I add the problems parameter with filterStatus "<value>"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		And the Bundle should be valid for patient "patient2"
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid 
		And check the response does not contain an operation outcome
Examples:  
	| value    |
	| active   |
	| inactive |

Scenario: Verify Investigations structured record for a Patient with Investigations associated to Problems
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient2"
		And I add the Investigations parameter
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		And the Bundle should be valid for patient "patient2"
	    And check that the bundle does not contain any duplicate resources
		And check the response does not contain an operation outcome
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid 
		And I Check the Investigations List is Valid
		And The Structured List Does Not Include Not In Use Fields	
		And I Check the DiagnosticReports are Valid
		And I Check the DiagnosticReports Do Not Include Not in Use Fields
		And I Check the Specimens are Valid		
		And I Check the Specimens Do Not Include Not in Use Fields
		And I Check The Problems Secondary Problems List
		And I Check The Problems Secondary Problems List Does Not Include Not In Use Fields
		And I Check The Problems Resources are Valid
		And I check The Problem Resources Do Not Include Not In Use Fields
		And Check a Problem is linked to DiagnosticReport and that it is also included
		And I Check the Test report Filing is Valid
		And I Check the Test report Filing Do Not Include Not in Use Fields
		And the Bundle should contain "2" lists

Scenario Outline: Retrieve the allergy structured record for a patient with problems linked but excluding resolved allergies
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "<Patient>"
		And I add the allergies parameter with resolvedAllergies set to "false"
	When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And the response should be a Bundle resource of type "collection"
		And the response meta profile should be for "structured"
		And the patient resource in the bundle should contain meta data profile and version id
		And if the response bundle contains a practitioner resource it should contain meta data profile and version id
		And if the response bundle contains an organization resource it should contain meta data profile and version id
		And the Patient Id should be valid
		And the Practitioner Id should be valid
		And the Organization Id should be valid
		And the Bundle should be valid for patient "<Patient>"
		And check that the bundle does not contain any duplicate resources
		And the Bundle should contain "2" lists
		And the Bundle should contain a list with the title "Allergies and adverse reactions"
		And the Bundle should not contain a list with the title "Ended allergies"
		And the AllergyIntolerance should be valid
		And the Bundle should contain the correct number of allergies
		And the Lists are valid for a patient with allergies
		And check the response does not contain an operation outcome
		And I Check The Problems Secondary Problems List
		And I Check The Problems Secondary Problems List Does Not Include Not In Use Fields
		And I Check The Problems Resources are Valid
		And I check The Problem Resources Do Not Include Not In Use Fields
		And Check a Problem is linked to an "AllergyIntolerance" that is also included in the response with its list

	Examples:
		| Patient   |
		| patient2  |

Scenario: Structured request sent for consultations and problems expect success
	Given I configure the default "GpcGetStructuredRecord" request
		And I add an NHS Number parameter for "patient2"
		And I add the includeConsultations parameter only
		And I add the Problems parameter
    When I make the "GpcGetStructuredRecord" request
	Then the response status code should indicate success
		And check that the bundle does not contain any duplicate resources
		And the patient resource in the bundle should contain meta data profile and version id
		And check the response does not contain an operation outcome		
		#Consultations Checks Below
		And I Check the Consultations List is Valid
		And The Consultations List Does Not Include Not In Use Fields
		And I Check the Encounters are Valid
		And I Check the Encounters Do Not Include Not in Use Fields
		And I Check the Consultation Lists are Valid
		And I Check All The Consultation Lists Do Not Include Not In Use Fields
		And I Check the Topic Lists are Valid
		And I Check one Topic is linked to a problem
		And I Check the Heading Lists are Valid	
		And I Check The Problems Resources are Valid
		And I check The Problem Resources Do Not Include Not In Use Fields
		And I Check that a Topic or Heading is linked to an "Observation" and that is included in response with a list
		And I Check that a Topic or Heading is linked to an "MedicationRequest" and that is included in response with a list
		And Check the MedicationRequests have a link to a medication that has been included in response
		And Check there is a MedicationStatement resource that is linked to the MedicationRequest and Medication
		And I Check the Consultation Medications Secondary List is Valid
		And I Check the Consultation Uncategorised Secondary List is Valid
		#Problems Checks Below
		And I Check The Primary Problems List
		And I Check The Primary Problems List Does Not Include Not In Use Fields
		And I Check The Problems Resources are Valid
		And I check The Problem Resources Do Not Include Not In Use Fields
		And Check a Problem is Linked to a MedicationRequest resource that has been included in the response
		And Check the MedicationRequests have a link to a medication that has been included in response
		And Check there is a MedicationStatement resource that is linked to the MedicationRequest and Medication
		And Check a Problem is linked to an "Observation" that is also included in the response with its list
		And I Check the Problems Uncategorised Secondary List is Valid
		And I Check the Problems Medications Secondary List is Valid

Scenario: Migrate Patient2 With Sensitive and then migrate first document
	Given I configure the default "MigrateStructuredRecordWithSensitive" request
	And I add an NHS Number parameter for "patient2"
		And I add the includeFullrecord parameter with includeSensitiveInformation set to "true"
    When I make the "MigrateStructuredRecordWithSensitive" request
	Then the response status code should indicate success
		And check that the bundle does not contain any duplicate resources
		And the patient resource in the bundle should contain meta data profile and version id
		And check the response does not contain an operation outcome
		And I Check Documents have been Returned and save the first documents url for retrieving later
		And I Check the returned DocumentReference is Valid
		And I Check the returned DocumentReference Do Not Include Not In Use Fields
	Given I configure the default "MigrateDocument" request
		When I make the "MigrateDocument" request
		Then the response status code should indicate success
		And I Check the returned Binary Document is Valid
		And I Check the returned Binary Document Do Not Include Not In Use Fields
		And I save the binary document from the retrieve

Scenario: Migrate Patient2 Without Sensitive and then migrate first document
	Given I configure the default "MigrateStructuredRecordWithoutSensitive" request
		And I add an NHS Number parameter for "patient2"
		And I add the includeFullrecord parameter with includeSensitiveInformation set to "false"
    When I make the "MigrateStructuredRecordWithoutSensitive" request
	Then the response status code should indicate success
		And check that the bundle does not contain any duplicate resources
		And the patient resource in the bundle should contain meta data profile and version id
		And check the response does not contain an operation outcome
		And I Check Documents have been Returned and save the first documents url for retrieving later
		And I Check the returned DocumentReference is Valid
		And I Check the returned DocumentReference Do Not Include Not In Use Fields
	Given I configure the default "MigrateDocument" request
		When I make the "MigrateDocument" request
		Then the response status code should indicate success
		And I Check the returned Binary Document is Valid
		And I Check the returned Binary Document Do Not Include Not In Use Fields
		And I save the binary document from the retrieve

Scenario Outline: Foundations CapabilityStatement returns correct profile versions
Given I configure the default "MetadataRead" request
	When I make the "MetadataRead" request
	Then the response status code should indicate success
	And the CapabilityStatement REST Operations should contain "gpc.getstructuredrecord"
    And the CapabilityStatement Profile should contain the correct reference and version history "<urlToCheck>" 
Examples: 
| urlToCheck                                                                              |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1                  |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1             |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1             |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-PractitionerRole-1         |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1                 |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1               |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Appointment-1                    |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Schedule-1                       |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1                           |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-AllergyIntolerance-1       |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Medication-1               |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationStatement-1      |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationRequest-1        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-List-1                     |
| https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-StructuredRecord-Bundle-1        |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Immunization-1             |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ProblemHeader-Condition-1  |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Encounter-1                |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Observation-1              |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-DiagnosticReport-1         |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Specimen-1			      |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ProcedureRequest-1         |
| https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-ReferralRequest-1          |