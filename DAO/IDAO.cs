using System.Collections.Generic;
using DTO;

namespace DAO
{
    public interface IChildDAO
    {
        List<Child> GetChildren(long id=-1L, string first_name="", string vesta_id="",
            Gender gender=Gender.NotSpecified);
        long AddSingleChild(Child c);
        int AddMultipleChildren(List<Child> children);
    }
    public interface IDonorDAO
    {
        List<Donor> GetDonors(long donorid = -1L, string name = null, string code = null);
        long AddSingleDonor(Donor d);
        int AddMultipleDonors(List<Donor> donors);
    }
    public interface IHeadOfHouseholdDAO
    {
        List<HeadOfHousehold> GetHeadOfHouseholds(string where_clause = "");
        long AddSingleHeadOfHousehold(HeadOfHousehold h);
        int AddMultipleHeadsOfHousehold(List<HeadOfHousehold> hohs);
    }
    // When we fetch the remaining items, we will always filter them by year.
    // Over time, there may well be many thousands of items of these types --
    // restricting the search to a single year will probably avoid any likelihood
    // of running out of RAM.
    public interface IServicesEnrollmentDAO
    {
        List<ServicesEnrollment> GetServicesEnrollments(int year, string where_clause = "");
        long AddSingleServicesEnrollment(ServicesEnrollment e);
        int AddMultipleServicesEnrollments(List<ServicesEnrollment> enrollments);
    }
    public interface ILabelEvent_BagDAO
    {
        List<LabelEvent_Bag> GetLabelEvents_Bag(int year, string where_clause = "");
        long AddSingleLabelEvent_Bag(LabelEvent_Bag e);
        int AddMultipleLabelEvents_Bag(List<LabelEvent_Bag> events);
    }
    public interface ILabelEvent_GiftDAO
    {
        List<LabelEvent_Gift> GetLabelEvents_Gift(int year, string where_clause = "");
        long AddSingleLabelEvent_Gift(LabelEvent_Gift e);
        int AddMultipleLabelEvents_Gift(List<LabelEvent_Gift> events);
    }
}
