namespace Domain.User
{
    public class UserMng
    {
        private readonly IUserRepository _userRepository;

        public UserMng(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Load(string userName)
        {
            return _userRepository.Load(userName) ?? _userRepository.Create();
        }
    }
}
