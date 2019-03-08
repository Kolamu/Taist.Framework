package Mock.Data.Helpers;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;

public class FileHelper {
	
	public static ArrayList<String> GetAllFileNames(String dirName, String extension){
		ArrayList<String> fileNameList = new ArrayList<String>();
		File directory = new File(dirName);
		if(!directory.exists()) return fileNameList;
		
		File[] childFileArray = directory.listFiles();
		if(childFileArray == null) {
			System.out.println("child file array is null");
			return fileNameList;
		}
		for(File childFile : childFileArray) {
			if(childFile.isDirectory()) {
				fileNameList.addAll(GetAllFileNames(childFile.getAbsolutePath(), extension));
			}
			else {
				if(childFile.getName().endsWith(extension)) {
					fileNameList.add(childFile.getAbsolutePath());
				}
			}
		}
		
		return fileNameList;
	}
	
	public static boolean Exists(String fileName) {
		return new File(fileName).exists();
	}
	
	/**
	 * Get *.txt file encoding format
	 * 
	 * *.txt contains 4 encoding format ANSI | Big endian(UTF-16) | Little endian(Unicode) | UTF-8
	 * 
	 * - ANSI - 0xXX 0xXX 0xXX
	 * - Unicode - 0xFF 0xFE
	 * - UTF-16 - 0xFE 0xFF
	 * - UTF-8 - 0xEF 0xBB 0xBF
	 * 
	 * @param fileName
	 * @return
	 */
	public static String getTXTFileEncoding(String fileName) {
		try {
			FileInputStream input = new FileInputStream(new File(fileName));
			byte[] b = new byte[3];
			int n = input.read(b);
			input.close();
			if(n < 2) {
				return "GBK";
			}
			
			if(b[0] == -1 && b[1] == -2) {
				return "Unicode";
			}
			
			if(b[0] == -2 && b[1] == -1) {
				return "UTF-16";
			}
			
			if(n < 3) {
				return "GBK";
			}
			if(b[0] == -17 && b[1] == -69 && b[2] == -65) {
				return "UTF-8";
			}
			
			return "GBK";
		} catch (Exception e) {
			e.printStackTrace();
			return "GBK";
		}
	}

	public static void createFile(String path) {
		File file = new File(path);
		if(file.exists()) {
			return;
		}
		
		File directory = new File(file.getParent());
		if(!directory.exists()) {
			directory.mkdirs();
		}
		
		try {
			file.createNewFile();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
