package Mock.Data.Helpers;

import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public final class StringHelper {
	public static boolean isNullOrEmpty(String value) {
		if(value == null) {
			return true;
		}
		
		if(value.isEmpty()) {
			return true;
		}
		
		return false;
	}
	
	public static boolean equals(String val1, String val2) {
		return equals(val1, val2, false);
	}
	
	public static boolean equals(String val1, String val2, boolean ignoreCase) {
		if(val1 == val2) {
			return true;
		}
		
		if(val1 == null || val2 == null) {
			return false;
		}
		
		if(ignoreCase) {
			return val1.toLowerCase().equals(val2.toLowerCase());
		}
		else {
			return val1.equals(val2);
		}
	}
	
	public static String replace(String regexPattern, String content, String newValue) {
		Pattern pattern = Pattern.compile(regexPattern);
		Matcher matcher = pattern.matcher(content);
		return matcher.replaceAll(newValue);
	}
	
	public static List<String> parseBhList(String bhString)
    {
        List<String> bhlist = new LinkedList<String>();
        if (bhString == null) {
            return bhlist;
        }
        String[] bhArray = bhString.split(",");
        for(String bh : bhArray) {
        	if(bh.contains("-")) {
        		bhlist.addAll(parseRangeBh(bh.trim()));
        	}
        	else {
        		bhlist.add(bh.trim());
        	}
        }
        return bhlist;
    }

	private static List<String> parseRangeBh(String rangeBhString) {
		if(rangeBhString.indexOf('-') == rangeBhString.lastIndexOf('-')) {
			String[] bhArray = rangeBhString.split("-");
			if(bhArray[0].trim().length() == bhArray[1].trim().length()) {
				return parseRangeBh(bhArray[0], bhArray[1]);
			}
		}
		List<String> bhlist = new LinkedList<String>();
		bhlist.add(rangeBhString);
		return bhlist;
	}
	
	private static List<String> parseRangeBh(String left, String right) {
		String lpfx = getBhPrifx(left.trim());
		String rpfx = getBhPrifx(right.trim());
		if(StringHelper.equals(lpfx, rpfx)) {
			return parseRangeBh(
					lpfx,
					left.trim().substring(lpfx.length()),
					right.trim().substring(lpfx.length()));
		}
		else {
			List<String> bhlist = new LinkedList<String>();
			bhlist.add(left + "-" + right);
			return bhlist;
		}
	}
	
	private static List<String> parseRangeBh(String prifx, String left, String right) {
		List<String> bhlist = new LinkedList<String>();
		int lnum = Integer.valueOf(left);
		int rnum = Integer.valueOf(right);
		if(lnum > rnum) {
			bhlist.add(prifx + left);
			bhlist.add(prifx + right);
		}
		else {
			while(lnum < rnum + 1) {
				bhlist.add(prifx+String.format("%0" + left.length() + "d", lnum));
				lnum++;
			}
		}
		return bhlist;
	}
	
    private static String getBhPrifx(String bhString) {
        int charIndex = bhString.length() - 1;
        while (charIndex > -1) {
            char c = bhString.charAt(charIndex);
            if (c < '0' || c > '9') {
                break;
            }
            charIndex--;
        }

        return bhString.substring(0, charIndex);
    }
}
