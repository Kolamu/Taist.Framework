package Mock.Data.Annotations;

import static java.lang.annotation.ElementType.PACKAGE;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target(PACKAGE)
@Retention(RetentionPolicy.RUNTIME)
public @interface BusinessPackage {
	
}
