# Exercise Notes

## General Notes

In the interest of SRP I've split the implementation into reader and writer classes, marking CSVReaderWriter as obsolete, with the suggestion that consumers would take dependencies on the new implementations in future.
So CSVReaderWriter now only exists to appease its current consumers, as an orchestrator of ICSVReader and ICSVWriter implementations. There doesn't therefore seem to be any need to refactor that class much further, as weirdies like the Mode enum and the "Unknown file mode for" exception will simply stop being used as the new implementations are adopted. Any further refactorings should occur in CSVReader and CSVWriter.

### Git
As git was mentioned in the readme, and a .gitignore was present, I took it as a hint and created a repo. This is actually quite handy for illustrating the steps in a refactoring assignment - I've committed against the tasks 1, 2 and 3 below, please do checkout any of those commits to see corresponding progress at each stage.

### StreamReader state
I ummed and ahhed a fair bit about this. I don't love that the implementation allows a consumer to leave the stream open, leaving us entirely at the mercy of the caller as to whether they remember to close it. 
I considered various options including opening and closing on each access. I'm torn between ensuring robustness and maintaining backwards compatibility. The latter wins - we're told there is production code (for instance AddressFileProcessor) which is expecting to call open and close. A major change in that sort of behaviour is going to need a conversation with the engineers of consuming code, so has been excluded from further refactorings in this exercise. 
Anyway, I think if we consider the CSVReader and CSVWriter to be relatively thin wrappers around the IO streams, it's not unreasonable to expect a consumer to control opening and closing, as is also true of consumers of the StreamReader itself, for instance. We also save on the performance overhead associated with creating connections, "well-performing code" being one of our goals here.

### Casing
The production code depends on CSVReaderWriter with the acronym capitalised. So although it goes against MS guidelines we can't change it without breaking something that uses it upstream. Inconsistency is worse in my mind than unusual casing, so I've continued to use the capitalized acronym.

## Tasks

### Task 1 
Job one before full refactor is to make the original code testable, without breaking its publicly facing contract. This way we'll have confidence that production code that uses it won't be broken by our upcoming changes. 

I'm using poor mans DI to bring in System.IO.Abstractions to replace calls to the filesystem. It should behave in the same way as before, except that it's testable. In the real world we'd probably use an IoC container for DI.

I'm slightly loath at this stage to call into the Open and Close methods in each unit test. This is ok in these integration style tests but later we'll move to test individual units of functionality at a time.

### Task 2
Job two is to begin properly refactoring the code. First thing is to move the two apparent responsibilities of CsvReaderWriter - reading and writing - into their own classes so as to obey SRP. From there I can tackle refactoring them separately, and test coverage will be simpler. The unit tests from task 1 now become integration tests, using actual implementations of the new CsvWriter and CsvReader, to make sure we're not breaking anything. The tests should still pass. New tests are added to cover the new implementations as well.

### Task 3
Job three is to refactor the two new implementations and their tests. All tests should still pass, new ones can be added. 
By moving the dependency on IFileSystem from CSVReaderWriter to the constructors of the new implementations and making the abstract TextReader / TextWriter accessible we can begin to move away from opening and closing the readers within the same test, which helps us move to testing smaller 'units' of functionality at a time.

##### Notes on refactor in task 3:

The parameters of method with signature "Read(string column1, string column2)" will behave like value types so don't get updated to the values from the StreamReader. I don't think it's possible for columns.Length == 0 so the predicate based on that is removed (see https://dotnetfiddle.net/J5jl6c). All that is left that the method does now is return true or fall over when the read line is misshaped. I'm going to take a punt and say it wasn't designed for this so have removed it from the ICsvReader interface, and it needs to be rewritten with the appropriate requirements to hand. Tests Should_throw_when_reading_incorrectly_formatted_file and Should_throw_when_reading_empty_file are removed also at this point.
Code that consumes this method may fail, but I'd suggest it's better to fail fast than to allow poorly written code to cause irregular or inconspicous failures.

Also I'm going to remove Should_throw_when_reading_out_incorrectly_formatted_file from Task 2. The lack of a custom exception suggests that it's unexpected, and it's behaviour should probably be more like when the line read is empty, where it returns false. The test becomes Should_return_false_when_reading_out_incorrectly_formatted_file, and the integration test is updated as well.

## Summary

Given we know the code already works, and is consumed in a production environment, it seems important to be careful in drawing the line when making assumptions about what the code should do. For instance I've said that one of the Read implementations isn't working correctly, but it's possible that an upstream component is actually expecting it to work in that incorrect way. Further refactorings will probably require either a conversation with consumers or inspection of inital requirement documentation.

Similarly one needs to draw a line to avoid over-engineering, and to distinguish between refactoring and simply making changes. Hopefully the remit has been reached in an understandable way so this seems like a good place to call the exercise complete.

Mike