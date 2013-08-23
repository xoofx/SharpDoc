package docLink;

import java.util.Comparator;


/**
 * Class that allow to compare ProjectFile to sort them by alphabetical order
 */
public class ProjectFileComparator implements Comparator<ProjectFile> {

	public ProjectFileComparator()
	{
	}

	/**
	 * Compare alphabetically the 'name' of the two given ProjectFiles
	 */
	@Override
	public int compare(ProjectFile file1, ProjectFile file2)
	{
		return file1.name.compareTo(file2.name);
	}

}
