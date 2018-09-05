namespace DAO
{
    public static class InitialDonors
    {
        public static Donor[] InitialList()
        {
            return new Donor[] {
                new Donor("EXCode1", "Example Donor Organization 1"),
                new Donor("EXCode2", "Example Donor Organization 2"),
                new Donor("GFTCRD", "Gift Cards")
            };
        }
    }
}

