namespace GPConnect.Provider.AcceptanceTests.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hl7.Fhir.Model;
    using GPConnect.Provider.AcceptanceTests.Helpers;

    public class FhirResponse
    {
        public Resource Resource { get; set; }

        public List<Bundle.EntryComponent> Entries
        {
            get
            {
                var r = Resource;
                if (r is Bundle b)
                    return b.Entry;

                if (r is OperationOutcome oo)
                    NUnit.Framework.Assert.Fail(
                        "Expected Bundle entries but got OperationOutcome:\n" +
                        oo.ToMultilineString()
                    );

                NUnit.Framework.Assert.Fail("Expected Bundle but got " + (r?.TypeName ?? "<null>"));
                return null; // never reached, satisfies compiler
    }
}
        public List<Patient> Patients => GetResources<Patient>();
        public List<Organization> Organizations => GetResources<Organization>();
        public List<Composition> Compositions => GetResources<Composition>();
        public List<Device> Devices => GetResources<Device>();
        public List<Practitioner> Practitioners => GetResources<Practitioner>();
        public List<Location> Locations => GetResources<Location>();
        public Bundle Bundle
        {
            get
            {
                var r = Resource; // or the backing field you already use
                if (r is Bundle b) return b;
                if (r is OperationOutcome oo)
                    NUnit.Framework.Assert.Fail("Expected Bundle but got OperationOutcome: " +
                                oo.Issue?.FirstOrDefault()?.Diagnostics);
                NUnit.Framework.Assert.Fail("Expected Bundle but got " + (r?.TypeName ?? "<null>"));
                return null; // Unreachable after Assert.Fail, keeps compiler happy
            }
        }
        public List<Slot> Slots => GetResources<Slot>();
        public List<Appointment> Appointments => GetResources<Appointment>();
        public List<Schedule> Schedules => GetResources<Schedule>();
        public List<CapabilityStatement> CapabilityStatements => GetResources<CapabilityStatement>();
        public List<AllergyIntolerance> AllergyIntolerances => GetResources<AllergyIntolerance>();
        public List<Medication> Medications => GetResources<Medication>();
        public List<MedicationStatement> MedicationStatements => GetResources<MedicationStatement>();
        public List<MedicationRequest> MedicationRequests => GetResources<MedicationRequest>();
        public List<List> Lists => GetResources<List>();
        public List<Immunization> Immunizations => GetResources<Immunization>();
        public List<Observation> Observations => GetResources<Observation>();
        public List<Encounter> Encounters => GetResources<Encounter>();
        public List<Condition> Conditions => GetResources<Condition>();
        public List<DiagnosticReport> DiagnosticReports => GetResources<DiagnosticReport>();
        public List<ReferralRequest> ReferralRequests => GetResources<ReferralRequest>();
        public List<ProcedureRequest> ProcedureRequests => GetResources<ProcedureRequest>();
        public List<Specimen> Specimens => GetResources<Specimen>();
        public List<DocumentReference> Documents => GetResources<DocumentReference>();
        public Binary BinaryDocument => (Binary)Resource;

        private List<T> GetResources<T>() where T : Resource
        {
            var type = typeof(T);

            // If the top-level is a Bundle, filter entries
            if (Resource.ResourceType == ResourceType.Bundle)
            {
                // If we have a map from CLR type -> FHIR ResourceType enum, use it
                if (ResourceTypeMap != null && ResourceTypeMap.TryGetValue(type, out var mappedRt))
                {
                    return Entries
                        .Where(e => e.Resource != null && e.Resource.ResourceType == mappedRt)
                        .Select(e => (T)e.Resource)
                        .ToList();
                }

                // Fallback if type isn't in the map: use a safe type check
                return Entries
                    .Where(e => e.Resource is T)
                    .Select(e => (T)e.Resource)
                    .ToList();
            }

            // Not a bundle: attempt to cast the single resource to T
            try
            {
                return new List<T> { (T)Resource };
            }
            catch (InvalidCastException)
            {
                // If the server returned an OperationOutcome, include its details
                if (Resource is OperationOutcome oo)
                {
                    NUnit.Framework.Assert.Fail(
                        "Expected " + typeof(T).Name + " but the server returned an OperationOutcome:\n" +
                        oo.ToMultilineString()
                    );
                }

                // Otherwise, keep the original exception behavior
                throw;
            }
        }


        private static Dictionary<Type, ResourceType> ResourceTypeMap => new Dictionary<Type, ResourceType>
        {
            {typeof(Patient), ResourceType.Patient},
            {typeof(Organization), ResourceType.Organization},
            {typeof(Composition), ResourceType.Composition},
            {typeof(Device), ResourceType.Device},
            {typeof(Practitioner), ResourceType.Practitioner},
            {typeof(Location), ResourceType.Location},
            {typeof(Slot), ResourceType.Slot},
            {typeof(Appointment), ResourceType.Appointment},
            {typeof(Schedule), ResourceType.Schedule},
            {typeof(CapabilityStatement), ResourceType.CapabilityStatement},
            {typeof(AllergyIntolerance), ResourceType.AllergyIntolerance},
            {typeof(List), ResourceType.List},
            {typeof(Medication), ResourceType.Medication},
            {typeof(MedicationStatement), ResourceType.MedicationStatement},
            {typeof(MedicationRequest), ResourceType.MedicationRequest},
            {typeof(Immunization), ResourceType.Immunization},
            {typeof(Observation), ResourceType.Observation},
            {typeof(Encounter), ResourceType.Encounter},
            {typeof(Condition), ResourceType.Condition},
            {typeof(DiagnosticReport), ResourceType.DiagnosticReport},
            {typeof(ReferralRequest), ResourceType.ReferralRequest},
            {typeof(ProcedureRequest), ResourceType.ProcedureRequest},
            {typeof(Specimen), ResourceType.Specimen},
            {typeof(DocumentReference), ResourceType.DocumentReference}

        };
    }
}
