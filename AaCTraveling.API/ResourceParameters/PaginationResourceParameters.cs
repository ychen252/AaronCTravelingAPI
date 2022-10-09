using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AaCTraveling.API.ResourceParameters
{
    public class PaginationResourceParameters
    {
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value >= 1)
                {
                    _pageNumber = value;
                }
            }
        }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value >= 1)
                {
                    _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
                }
            }
        }
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private const int MAX_PAGE_SIZE = 50;
    }
}
