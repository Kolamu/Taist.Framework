package Mock.Data.Internal;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.net.URLClassLoader;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.List;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

import Mock.Data.Annotations.BusinessClass;
import Mock.Data.Annotations.BusinessPackage;
import Mock.Data.Annotations.DataClass;
import Mock.Data.Annotations.DataPackage;
import Mock.Data.Helpers.FileHelper;
import Mock.Exception.CanNotFindTypeException;

public final class TaistPackage {
	private String jarPath;
	private HashMap<String, TaistType> typeMap = null;
	private TaistPackage nextPackage = null;
	private URLClassLoader loader = null;
	private ArrayList<String> classNameList = null;
	
	private static HashMap<String, TaistPackage> packageCache = null;
	
	private TaistPackage() {
	}
	
	public static TaistPackage getPackage(String baseDirName) {
		TaistPackage head = getCachedPackage(baseDirName);
		if(head != null) {
			return head;
		}
		
		ArrayList<String> jarFileNameList = FileHelper.GetAllFileNames(baseDirName, ".jar");
		
		head = new TaistPackage();
		if(jarFileNameList.isEmpty()) {
			head.jarPath = baseDirName;
			head.cacheClassName();
		}
		else {
			TaistPackage current = head;
			for(String jarFileName : jarFileNameList) {
				current.jarPath = jarFileName;
				current.cacheClassName();
				current = current.nextPackage;
			}
		}
		cachePackage(baseDirName, head);
		return head;
	}

	public void registerClass(Class<?> baseType) {
		if(classNameList == null) {
			classNameList = new ArrayList<String>();
		}
		classNameList.add(baseType.getName());
		cacheType(baseType.getName(), getType(baseType));
	}
	
	public TaistType getTaistType(String typeName) throws CanNotFindTypeException {
		try {
			String realTypeName = getRealTypeName(typeName);
			
			TaistType type = getCachedType(realTypeName);
			if(type != null) {
				return type;
			}
			
			loadPackage();
			
			type = getType(realTypeName);
			if(type == null) {
				throw new CanNotFindTypeException(typeName);
			}
			else {
				cacheType(realTypeName, type);
			}
			return type;
		}catch(CanNotFindTypeException e) {
			if(nextPackage == null) {
				throw e;
			}
			else {
				return nextPackage.getTaistType(typeName);
			}
		}
	}
	
	public ArrayList<String> getClassNameList(){
		ArrayList<String> allClassNameList = new ArrayList<>();
		
		allClassNameList.addAll(classNameList);
		if(nextPackage != null) {
			allClassNameList.addAll(nextPackage.getClassNameList());
		}
		
		return allClassNameList;
	}
	
	private static TaistPackage getCachedPackage(String baseDirName) {
		if(packageCache == null) {
			return null;
		}
		if(packageCache.containsKey(baseDirName)) {
			return packageCache.get(baseDirName);
		}
		else {
			return null;
		}
	}
	
	private static void cachePackage(String baseDirName, TaistPackage pack) {
		if(packageCache == null) {
			packageCache = new HashMap<String, TaistPackage>();
		}
		
		packageCache.put(baseDirName, pack);
	}

	private String getRealTypeName(String name) throws CanNotFindTypeException {
		if(classNameList == null) {
			throw new CanNotFindTypeException(name);
		}
		for(String className : classNameList) {
			if(className.endsWith(name)) {
				return className;
			}
		}
		throw new CanNotFindTypeException(name);
	}
	
	private String getTypeName(JarEntry entry) {
		try {
			String entryName = entry.getName();
			if(entryName.endsWith(".class") && !entryName.endsWith("package-info.class")) {
				entryName = entryName.substring(0, entryName.length()-6);
				return entryName.replace('/', '.');
			} else {
				return "";
			}
		}
		catch(Exception e) {
			return "";
		}
	}
	
	private void loadPackage() {
		if(loader != null) return;
		if(jarPath == null) return;
		try {
			File jar = new File(jarPath);
			loader = new URLClassLoader(new URL[] {
						jar.toURI().toURL()
					}, Thread.currentThread().getContextClassLoader());
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private TaistType getType(String realTypeName) {
		try {
			return getType(loader.loadClass(realTypeName));
		} catch (ClassNotFoundException e) {
			return null;
		}
	}
	
	private TaistType getType(Class<?> type) {
		Package pack = type.getPackage();
		if(pack.isAnnotationPresent(BusinessPackage.class) && type.isAnnotationPresent(BusinessClass.class)) {
			return new BusinessType(type);
		}
		else if(pack.isAnnotationPresent(DataPackage.class) && type.isAnnotationPresent(DataClass.class)) {
			return new DataType(type);
		}
		else {
			return new UnknownType(type);
		}
	}
	
	private TaistType getCachedType(String typeName) {
		if(typeMap == null) {
			return null;
		}
		
		if(typeMap.containsKey(typeName)) {
			return typeMap.get(typeName);
		}
		else {
			return null;
		}
	}
	
	private void cacheType(String typeName, TaistType type) {
		if(typeMap == null) {
			typeMap = new HashMap<String, TaistType>();
		}
		
		typeMap.put(typeName, type);
	}
	
	private void cacheClassName() {
		if(classNameList == null) {
			classNameList = new ArrayList<>();
		}
		File file = new File(jarPath);
		if(file.isDirectory()) {
			cacheDirectory(file);
		}
		else if(file.getPath().toLowerCase().endsWith(".jar")){
			cacheJarFile(file);
		}
	}
	
	private void cacheJarFile(File file) {
		JarFile jar = null;
		try {
			
			jar = new JarFile(file);
			
			Enumeration<JarEntry> jEntries = jar.entries();
			
			while(jEntries.hasMoreElements()) {
				String className = getTypeName(jEntries.nextElement());
				if(!className.isEmpty()) {
					classNameList.add(className);
				}
			}
		} catch (IOException e) {
		} finally {
			if(jar != null) {
				try {
					jar.close();
				} catch (IOException e) {
				}
			}
		}
	}
	
	private void cacheDirectory(File file) {
		List<String> fileList = FileHelper.GetAllFileNames(file.getAbsolutePath(), ".class");
		for(String fileName : fileList) {
			String className = fileName.substring(file.getAbsolutePath().length() + 1);
			className = className.substring(0, className.length() - 6).replace("\\", ".");
			classNameList.add(className);
		}
	}
	
	protected void finalize() throws Throwable {
		super.finalize();
		loader.close();
	}
}
