namespace next.core.entities
{
    internal class UserSearchQueryBo : ISearchIndexable
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? EstimatedRowCount { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? SearchProgress { get; set; }

        public string? StateCode { get; set; }

        public string? CountyName { get; set; }

        public string this[int index]
        {
            get
            {
                const string dfmt1 = "M/d/yy h:mm tt";
                const string dfmt2 = "M/d/yyyy";
                const string dash = " - ";
                if (index < 0 || index > 7) return string.Empty;
                if (index == 0) return Id ?? string.Empty;
                if (index == 1) return CreateDate.HasValue ? CreateDate.Value.ToString(dfmt1) : dash;
                if (index == 2) return StateCode ?? dash;
                if (index == 3) return CountyName ?? dash;
                if (index == 4) return StartDate.HasValue ? StartDate.Value.ToString(dfmt2) : dash;
                if (index == 5) return EndDate.HasValue ? EndDate.Value.ToString(dfmt2) : dash;
                if (index == 6) return ConvertStatus(SearchProgress);
                return string.Empty;
            }
        }

        private static string ConvertStatus(string? status)
        {
            const string dash = " - ";
            if (string.IsNullOrEmpty(status)) return dash;
            if (!status.Contains('-')) return status;
            return status.Split('-')[^1];
        }
    }
}
