﻿namespace GPConnect.Provider.AcceptanceTests.Steps
{
    using Context;
    using Hl7.Fhir.Model;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enum;
    using Repository;
    using TechTalk.SpecFlow;
    using Constants;

    [Binding]
    public class BookAppointmentSteps : BaseSteps
    {
        private readonly IFhirResourceRepository _fhirResourceRepository;
        private readonly HttpContext _httpContext;

        private List<Appointment> Appointments => _httpContext.FhirResponse.Appointments;

        public BookAppointmentSteps(HttpSteps httpSteps, HttpContext httpContext, IFhirResourceRepository fhirResourceRepository)
            : base(httpSteps)
        {
            _httpContext = httpContext;
            _fhirResourceRepository = fhirResourceRepository;
        }

        [Given(@"I add an Invalid Extension with Code only to the Created Appointment")]
        public void AddAnInvalidExtensionWithCodeOnlyToTheCreatedAppointment()
        {
            var coding = new Coding
            {
                Code = "TEL"
            };

            var codableConcept = new CodeableConcept();
            codableConcept.Coding.Add(coding);

            var extension = new Extension
            {
                Value = codableConcept
            };

            _fhirResourceRepository.Appointment.Extension.Add(extension);
        }

        [Given(@"I add an Invalid Extension with Url only to the Created Appointment")]
        public void AddAnInvalidExtensionWithUrlToTheCreatedAppointment()
        {
            var extension = new Extension
            {
                Url = "RandomExtensionUsedForTesting"
            };

            _fhirResourceRepository.Appointment.Extension.Add(extension);
        }

        [Given(@"I add an Invalid Extension with Url, Code and Display to the Created Appointment")]
        public void AddAnInvalidExtensionWithUrlCodeAndDisplayToTheCreatedAppointment()
        {
            var coding = new Coding
            {
                Code = "TEL",
                Display = "Telephone"
            };

            var codableConcept = new CodeableConcept();
            codableConcept.Coding.Add(coding);

            var extension = new Extension
            {
                Url = "http://fhir.nhs.uk/StructureDefinition/extension-gpconnect-appointment-notanextension-1",
                Value = codableConcept
            };

            _fhirResourceRepository.Appointment.Extension.Add(extension);
        }

        [Given(@"I set the Created Appointment Id to ""([^""]*)""")]
        public void SetTheCreatedAppointmentIdTo(string id)
        {
            _fhirResourceRepository.Appointment.Id = id;
        }
        
        [Given(@"I add an Appointment Reason to the appointment")]
        public void thenIAddAnAppointmentReasonToTheAppointment()
        {
            _fhirResourceRepository.Appointment.Reason.Add(new CodeableConcept("http://snomed.info/sct", "17436001", "Medical consultation with outpatient", "Medical consultation with outpatient"));
        }
// git hub ref 141
// 6/12/2018 RMB
        [Given(@"I add a typeCode to the Created Appointment")]
        public void IaddatypeCodetotheCreatedAppointment()
        {
            _fhirResourceRepository.Appointment.Specialty.Add(new CodeableConcept("http://hl7.org/fhir/stu3/valueset-c80-practice-codes", "394802001", "General medicine", null));
        }

// git hub ref 200
// RMB 20/2/19
        [Given(@"I amend the Organization reference to absolute reference")]
        public void Iamendtheorganizationreferencetoabsolutereference()
        {
            var organizationReference = new ResourceReference {Reference = "https://test1.supplier.thirdparty.nhs.uk/A11111/STU3/1/GPConnect/#1"};
            var arExt = new Extension(FhirConst.StructureDefinitionSystems.kAppointmentBookingOrganization, organizationReference);

            _fhirResourceRepository.Appointment.RemoveExtension(FhirConst.StructureDefinitionSystems.kAppointmentBookingOrganization);
            _fhirResourceRepository.Appointment.Extension.Add(arExt);
        }


        [Given(@"I set the Created Appointment Slot Reference to ""([^""]*)""")]
        public void SetTheCreatedAppointmentSlotReferenceTo(string slotReference)
        {
            var reference = new ResourceReference
            {
                Reference = slotReference
            };

            _fhirResourceRepository.Appointment.Slot.Clear();
            _fhirResourceRepository.Appointment.Slot.Add(reference);
        }

        [Given(@"I remove the Start from the Created Appointment")]
        public void RemoveTheStartFromTheCreatedAppointment()
        {
            _fhirResourceRepository.Appointment.Start = null;
        }

        [Given(@"I remove the End from the Created Appointment")]
        public void RemoveTheEndFromTheCreatedAppointment()
        {
            _fhirResourceRepository.Appointment.End = null;
        }

        [Given(@"I remove the Status from the Created Appointment")]
        public void RemoveTheStatusFromTheCreatedAppointment()
        {
            _fhirResourceRepository.Appointment.Status = null;
        }

        [Given(@"I remove the Slot from the Created Appointment")]
        public void RemoveTheSlotFromTheCreatedAppointment()
        {
            _fhirResourceRepository.Appointment.Slot = null;
        }

        [Given(@"I set the Created Appointment Identifier Value to null")]
        public void SetTheCreatedAppointmentIdentifierValueToNull()
        {
            var identifiers = new List<Identifier>
            {
                new Identifier(FhirConst.IdentifierSystems.kAppointment, null)
            };

            _fhirResourceRepository.Appointment.Identifier = identifiers;
        }

        [Given(@"I set the Created Appointment Patient Participant Status to null")]
        public void SetTheCreatedAppointmentPatientParticipantStatusToNull()
        {
            _fhirResourceRepository.Appointment.Participant.ForEach(participant =>
            {
                if (participant.Actor.Reference.ToString().Contains("Patient"))
                {
                    participant.Status = null;
                }
            });
        }

        [Given(@"I set the Created Appointment Practitioner Participant Status to null")]
        public void SetTheCreatedAppointmentPractitionerParticipantStatusToNull()
        {
            _fhirResourceRepository.Appointment.Participant.ForEach(participant =>
            {
                if (participant.Actor.Reference.ToString().Contains("Practitioner"))
                {
                    participant.Status = null;
                }
            });
        }

        [Given(@"I set the Created Appointment Location Participant Status to null")]
        public void SetTheCreatedAppointmentLocationParticipantStatusToNull()
        {
            _fhirResourceRepository.Appointment.Participant.ForEach(participant =>
            {
                if (participant.Actor.Reference.ToString().Contains("Location"))
                {
                    participant.Status = null;
                }
            });
        }

        [Given(@"I set the Created Appointment Participant Type Coding ""([^""]*)"" to null for ""([^""]*)"" Participants")]
        public void SetTheCreatedAppointmentParticipantTypeCodingToNullForParticipants(string codingType, string participantType)
        {
            _fhirResourceRepository.Appointment.Participant.ForEach(particpant =>
            {
                if (particpant.Actor.Reference.Contains(participantType))
                {
                    switch (codingType)
                    {
                        case "system":
                            if (particpant.Type.Count > 0)
                            {
                                particpant.Type.First().Coding.First().System = null;
                            }
                            break;
                        case "code":
                            if (particpant.Type.Count > 0)
                            {
                                particpant.Type.First().Coding.First().Code = null;
                            }
                            break;
                        case "display":
                            if (particpant.Type.Count > 0)
                            {
                                particpant.Type.First().Coding.First().Display = null;
                            }
                            break;
                    }
                }
            });
        }

        [Given(@"I remove the ""([^""]*)"" Participants from the Created Appointment")]
        public void RemoveTheParticipantsFromTheCreatedAppointment(string participantType)
        {
            _fhirResourceRepository.Appointment
                .Participant
                .RemoveAll(participant => participant.Actor.Reference.Contains(participantType));
        }

        [Then("the Appointment Cancellation Reason Extension should be valid")]
        public void TheAppointmentCancellationReasonExtensionShouldBeValid()
        {
            Appointments.ForEach(appointment =>
            {
                const string cancellationReasonUrl = FhirConst.StructureDefinitionSystems.kAppointmentCancellationReason;

                var appointmentCancellationReasonExtensions = appointment
                    .Extension
                    .Where(extension => extension.Url == cancellationReasonUrl)
                    .ToList();

                appointmentCancellationReasonExtensions.Count.ShouldBeLessThanOrEqualTo(1,
                    $"There should be a maximum of 1 Appointment Cancellation Reason Extensions but found {appointmentCancellationReasonExtensions.Count}");

                if (appointmentCancellationReasonExtensions.Any())
                {
                    appointmentCancellationReasonExtensions[0].Url.ShouldBeOfType<Uri>();
                    appointmentCancellationReasonExtensions[0].Value.ShouldBeOfType<string>();
                }
            });
        }

        [Then(@"the Appointment Location Participant should be valid and resolvable")]
        public void TheAppointmentLocationParticipantShouldBeValidAndResolvable()
        {
            Appointments.ForEach(appointment =>
            {
                appointment.Participant.ForEach(participant =>
                {
                    if (participant.Actor.Reference.StartsWith("Location/"))
                    {
                        var location = _httpSteps.GetResourceForRelativeUrl(GpConnectInteraction.LocationRead, participant.Actor.Reference);

                        location.ShouldNotBeNull(
                            $"The Appointment Participant with Reference {participant.Actor.Reference} returned a null Location.");
                        location.GetType()
                            .ShouldBe(typeof(Location),
                                $"The Appointment Participant with Reference {participant.Actor.Reference} returned a {location.GetType().ToString()}.");
                    }
                });
            });
        }

        [Then(@"the booking organization extension must be valid")]
        public void ThenTheBookedAppointmentExtensionMustBeValid()
        {
            Appointments.ForEach(appointment =>
            {
                var bookingOrgExtensions = appointment
                 .Extension
                 .Where(extension => extension.Url == "bookingOrganization")
                 .ToList();

                if (bookingOrgExtensions.Count != 0)
                {
                    appointment.Contained.ForEach(contained =>
                    {
                        Organization org = (Organization)contained;
                        org.Id.ShouldNotBeNull();
                        // Added 1.2.1 RMB 1/10/2018
                        org.Type.ShouldBeNull();
                        org.Name.ShouldNotBeNull();
                        org.Telecom.ShouldNotBeNull();

                    });
                }
            });

        }

        [Given(@"I remove the booking organization telecom element")]
        public void IRemoveTheBookingOrganizationTelecomElement()
        {
             _fhirResourceRepository.Appointment.Contained.ForEach(contained => 
            {
                Organization org = (Organization)contained;
                org.Telecom = null;
            });

        }

        [Given(@"I remove the booking organization name element")]
        public void IRemoveTheBookingOrganizationNameElement()
        {
            _fhirResourceRepository.Appointment.Contained.ForEach(contained =>
            {
                Organization org = (Organization)contained;
                org.Name = null;
            });

        }

    } 
}

