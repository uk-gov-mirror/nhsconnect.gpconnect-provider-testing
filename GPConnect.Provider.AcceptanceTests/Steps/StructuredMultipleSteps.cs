﻿namespace GPConnect.Provider.AcceptanceTests.Steps
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using TechTalk.SpecFlow;
	using GPConnect.Provider.AcceptanceTests.Steps;
	using Shouldly;
	using GPConnect.Provider.AcceptanceTests.Helpers;
	using GPConnect.Provider.AcceptanceTests.Logger;
	using Constants;
	using Context;
	using static Hl7.Fhir.Model.Parameters;
	using Hl7.Fhir.Model;


	[Binding]
	public sealed class StructuredMultipleSteps : BaseSteps
	{
		private readonly HttpContext _httpContext;

		public StructuredMultipleSteps(HttpSteps httpSteps, HttpContext httpContext)
			: base(httpSteps)
		{
			_httpContext = httpContext;
		}

		[Then(@"Check the operation outcome returns the correct text and diagnotics ""([^""]*)""")]
		public void checkTheOperationOutcomeReturnsTheCorrectTextAndDiagnostics(string parameters)
		{
			var entries = _httpContext.FhirResponse.Entries;
			entries.ForEach(entry =>
			{

				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{

					foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
					{
						issue.Code.ToString().ShouldBe("NotSupported");
						issue.Severity.ToString().ShouldBe("Warning");

						issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
						issue.Details.Coding[0].Code.ShouldBe("NOT_IMPLEMENTED");
						issue.Details.Coding[0].Display.ShouldBe("Not implemented");
						issue.Details.Text.ShouldContain(parameters + " is an unrecognised parameter");
						issue.Diagnostics.ShouldContain(parameters);

						Log.WriteLine("The response is not returning the expected value for " + parameters);

					}
				}
			});
		}
		[Then(@"Check the operation outcome for each unsupported structured request")]
		public void checkTheOperationOutcomeForEachUnsupportedStructuredRequest()
		{
			var entries = _httpContext.FhirResponse.Entries;
			string paramsToCheck = "includeImmunisations,includeProblems,includeConsultations,includeUncategorisedData,inventedParameter";

			var parameterList = paramsToCheck.Split(',');
			entries.ForEach(entry =>
			{

				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{
					//check number of issues in operation outcome matches expected number
					if (((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue.Count() == parameterList.Count())
					{


						foreach (var param in parameterList)
						{

							bool found = false;

							foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
							{
								if (issue.Details.Text == (param + " is an unrecognised parameter"))
								{
									issue.Code.ToString().ShouldBe("NotSupported");
									issue.Severity.ToString().ShouldBe("Warning");

									issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
									issue.Details.Coding[0].Code.ShouldBe("NOT_IMPLEMENTED");
									issue.Details.Coding[0].Display.ShouldBe("Not implemented");
									issue.Diagnostics.ShouldBe(param);


									found = true;
									break;
								}
							}

							if (!found)
							{
								Log.WriteLine("The operation outcome issue not found for:" + param);
								found.ShouldBeTrue("operation outcome issue not found for:" + param);
							}
							else
							{
								Log.WriteLine("operation outcome issue found for:" + param);

							}
						}
					}
					else
					{
						var message = "the number of Operational Outcome issues(" + ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue.Count().ToString() +
						") does not match expected(" + parameterList.Count().ToString() + ")";
						Log.WriteLine(message);
						((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue.Count().ShouldBe(parameterList.Count(), message);
					}
				}

			}
			);
		}

		[Then(@"Check the operation outcome returns the correct text and diagnostics ""([^""]*)"" and ""([^""]*)""")]
		public void checkTheOperationOutcomeReturnsTheCorrectTextAndDiagnostics(string parameter, string partparameter)
		{
			var entries = _httpContext.FhirResponse.Entries;

			entries.ForEach(entry =>
			{

				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{

					foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
					{

						issue.Code.ToString().ShouldBe("NotSupported");
						issue.Severity.ToString().ShouldBe("Warning");

						issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
						issue.Details.Coding[0].Code.ShouldBe("NOT_IMPLEMENTED");
						issue.Details.Coding[0].Display.ShouldBe("Not implemented");
						issue.Details.Text.ShouldBe(parameter + "." + partparameter + " is an unrecognised parameter");
						issue.Diagnostics.ShouldBe(parameter + "." + partparameter);
						Log.WriteLine("The response is not returning the expected value for " + parameter);

					}

				}
			});
		}

		[Then(@"Check the operation outcome returns PARAMETER_NOT_FOUND for Allergies")]
		public void checkTheOperationOutcomeReturnsParameterNotFoundForAllergies()
		{
			var entries = _httpContext.FhirResponse.Entries;
			var parameter = "includeAllergies";
			var partparameter = "includeResolvedAllergies";


			entries.ForEach(entry =>
			{

				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{

					foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
					{

						issue.Code.ToString().ShouldBe("Required");
						issue.Severity.ToString().ShouldBe("Warning");

						issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
						issue.Details.Coding[0].Code.ShouldBe("PARAMETER_NOT_FOUND");
						issue.Details.Coding[0].Display.ShouldBe("Parameter not found");
						issue.Details.Text.ShouldBe("Miss parameter part : " + partparameter);
						issue.Diagnostics.ShouldBe(parameter);
						Log.WriteLine("This has not met the expected response of PARAMETER_NOT_FOUND for " + parameter);

					}

				}
			});
		}
		
		// #311 14/10/2019 SJD removed as revision of spec
		//[Then(@"Check the operation outcome returns PARAMETER_NOT_FOUND for Medication")]
		//public void checkTheOperationOutcomeReturnsParameterNotFoundForMedication()
		//{
		//	var entries = _httpContext.FhirResponse.Entries;
		//	var parameter = "includeMedication";
		//	var partparameter = "includePrescriptionIssues";


		//	entries.ForEach(entry =>
		//	{

		//		if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
		//		{

		//			foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
		//			{

		//				issue.Code.ToString().ShouldBe("Required");
		//				issue.Severity.ToString().ShouldBe("Warning");

		//				issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
		//				issue.Details.Coding[0].Code.ShouldBe("PARAMETER_NOT_FOUND");
		//				issue.Details.Coding[0].Display.ShouldBe("Parameter not found");
		//				issue.Details.Text.ShouldBe("Miss parameter part : " + partparameter);
		//				issue.Diagnostics.ShouldBe(parameter);
		//				Log.WriteLine("This has not met the expected response of PARAMETER_NOT_FOUND for " + parameter);

		//			}

		//		}
		//	});
		//}

		[Then(@"Check the operation outcome returns INVALID_PARAMETER")]
		public void checkTheOperationOutcomeReturnsInvalid_Parameter()
		{
			var entries = _httpContext.FhirResponse.Entries;
			var parameter = "includeMedication";
			var partparameter = "medicationSearchFromDate";

			entries.ForEach(entry =>
			{

				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{

					foreach (var issue in ((Hl7.Fhir.Model.OperationOutcome)entry.Resource).Issue)
					{

						issue.Code.ToString().ShouldBe("Invalid");
						issue.Severity.ToString().ShouldBe("Warning");

						issue.Details.Coding[0].System.ShouldBe("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1");
						issue.Details.Coding[0].Code.ShouldBe("INVALID_PARAMETER");
						issue.Details.Coding[0].Display.ShouldBe("Invalid Parameter");
						issue.Details.Text.ShouldBe("Invalid date used");
						issue.Diagnostics.ShouldBe(parameter + "." + partparameter);
						Log.WriteLine("This has not met the expected response of INVALID_PARAMETER for " + parameter + "." + partparameter);

					}

				}
			});
		}


		[Given(@"The request only contains mandatory parameters")]
		public void GivenTheRequestOnlyContainsMandatoryParameters()
		{
			Given($"I add the allergies parameter with resolvedAllergies set to \"false\"");
			Given($"I add the medication parameter with includePrescriptionIssues set to \"false\"");
		}

		[Given(@"I send a request that contains all forward compatable structured parameters with optional parameters")]
		public void GivenISendARequestThatContainsAllForwardCompatibleStructuredParametersWithOptionalParameters()
		{
			Given($"I add the allergies parameter with resolvedAllergies set to \"true\"");
			Given($"I add the immunisations parameter");
			Given($"I add the uncategorised parameter with optional parameters");
			Given($"I add the consultation parameter with optional parameters");
			Given($"I add the problems parameter with optional parameters");

		}

		[Given(@"I add the immunisations parameter")]
		public void GivenIAddTheImmunisationsParameter()
		{
			ParameterComponent param = new ParameterComponent();
			param.Name = FhirConst.GetStructuredRecordParams.kImmunisations;
			_httpContext.HttpRequestConfiguration.BodyParameters.Parameter.Add(param);
		}

		[Given(@"I duplicate a parameter")]
		public void GivenIDuplicateParameter()
		{
			ParameterComponent param = new ParameterComponent();
			param.Name = FhirConst.GetStructuredRecordParams.kImmunisations;
			_httpContext.HttpRequestConfiguration.BodyParameters.Parameter.Add(param);

			ParameterComponent param1 = new ParameterComponent();
			param1.Name = FhirConst.GetStructuredRecordParams.kImmunisations;
			_httpContext.HttpRequestConfiguration.BodyParameters.Parameter.Add(param1);
		}

		[Given(@"I add allergies parameter with invalid ""(.*)""")]
		public void GivenIAddAllergiesParameterWithInvalid(string rubbishPartParameter)
		{
			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[]
			{
				Tuple.Create("RubbishPartParameter", (Base)new FhirBoolean(true))
				};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add(FhirConst.GetStructuredRecordParams.kAllergies, tuples);

		}

		[Given(@"I add allergies parameter with invalid part parameter boolean")]
		public void GivenIAddAllergiesParameterWithInvalidPartParameterBoolean()
		{
			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[]
			{
				Tuple.Create(FhirConst.GetStructuredRecordParams.kResolvedAllergies, (Base)new FhirBoolean(null))
				};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add(FhirConst.GetStructuredRecordParams.kAllergies, tuples);

		}

		[Given(@"I add the uncategorised parameter with optional parameters")]
		public void GivenIAddTheUncategorisedParameterWithOptionalParameters()
		{
			var backDate = DateTime.UtcNow.AddDays(-10);
			var futureDate = DateTime.UtcNow.AddDays(5);
			var startDate = backDate.ToString("yyyy-MM-dd");
			var endDate = futureDate.ToString("yyyy-MM-dd");

			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[] {
				Tuple.Create(FhirConst.GetStructuredRecordParams.kUncategorisedData, (Base)FhirHelper.GetTimePeriod(startDate, endDate)),
			};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add(FhirConst.GetStructuredRecordParams.kUncategorised, tuples);
		}

		[Given(@"I add the consultation parameter with optional parameters")]
		public void GivenIAddTheConsultationParameterWithOptionalParameters()
		{
			var backDate = DateTime.UtcNow.AddDays(-10);
			var futureDate = DateTime.UtcNow.AddDays(5);
			var startDate = backDate.ToString("yyyy-MM-dd");
			var endDate = futureDate.ToString("yyyy-MM-dd");

			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[] {
				Tuple.Create(FhirConst.GetStructuredRecordParams.kConsultationSearch, (Base)FhirHelper.GetTimePeriod(startDate, endDate)),
				Tuple.Create(FhirConst.GetStructuredRecordParams.kConsultationsMostRecent, (Base)new FhirString("3"))
			};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add(FhirConst.GetStructuredRecordParams.kConsultations, tuples);
		}

		[Given(@"I send an invalid Consultations parameter containing valid part parameters")]
		public void GivenISendAnInvalidConsultationsParameterContainingValidPartParameters()
		{
			var backDate = DateTime.UtcNow.AddDays(-10);
			var futureDate = DateTime.UtcNow.AddDays(5);
			var startDate = backDate.ToString("yyyy-MM-dd");
			var endDate = futureDate.ToString("yyyy-MM-dd");

			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[] {
				Tuple.Create(FhirConst.GetStructuredRecordParams.kConsultationSearch, (Base)FhirHelper.GetTimePeriod(startDate, endDate)),
				Tuple.Create(FhirConst.GetStructuredRecordParams.kConsultationsMostRecent, (Base)new FhirString("3"))
			};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add("Consultations", tuples);
		}

		[Given(@"I add the problems parameter with optional parameters")]
		public void GivenIAddTheProblemsParameterWithOptionalParameters()
		{
			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[] {
				Tuple.Create(FhirConst.GetStructuredRecordParams.kProblemsStatus, (Base)new FhirString ("active")),
				Tuple.Create(FhirConst.GetStructuredRecordParams.kProblemsSignificance, (Base)new FhirString ("major"))
			};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add(FhirConst.GetStructuredRecordParams.kProblems, tuples);
		}

		[Given(@"I add 2 success and 2 unsupported with invalid parameters details")]
		public void GivenIAdd2InvalidParametersAndPartParameters()
		{

			Given($"I add the medication parameter with includePrescriptionIssues set to \"false\"");
			IEnumerable<Tuple<string, Base>> tuples = new Tuple<string, Base>[] {
				Tuple.Create("madeUp", (Base)new FhirString ("madeUpValue1")),
				Tuple.Create(FhirConst.GetStructuredRecordParams.kProblemsSignificance, (Base)new FhirString ("madeUpValue2"))
			};
			_httpContext.HttpRequestConfiguration.BodyParameters.Add("madeUpProblems", tuples);

			ParameterComponent param = new ParameterComponent();
			param.Name = "madeUpImmunisations";
			_httpContext.HttpRequestConfiguration.BodyParameters.Parameter.Add(param);
		}

		//SJD added 14/10/2019 to check for multiple operation outcomes 
		[Then(@"Check the number of issues in the operation outcome ""([^""]*)""")]
		public void checkTheNumberOfIssuesInTheOperationalOutcome(int issueCount)
		{
			var entries = _httpContext.FhirResponse.Entries;
			var foundFlag = false;
			entries.ForEach(entry =>
			{
				if (entry.Resource.ResourceType.ToString() == "OperationOutcome")
				{
					((OperationOutcome)entry.Resource).Issue.Count().ShouldBe(issueCount);
					foundFlag = true;

				}

			});
			if (!foundFlag)
			{
				var message = "Test failed as NO operation outcome found in the response";
				Log.WriteLine(message);
				foundFlag.ShouldBeTrue(message);
			}
		}
	}
}

