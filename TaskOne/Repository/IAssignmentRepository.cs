using TaskOne.Models;

namespace TaskOne.Repository
{
    public interface IAssignmentRepository
    {
        Assignments CreateAssignment(Assignments assignments);
        void SaveFilePaths(List<FilePath> filePathlist);
        List<Assignments> GetAllAssignments();
        List<FilePath> GetFilesbyAssignmentId(int id);
    }
}
