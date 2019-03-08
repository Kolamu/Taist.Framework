package Mock.Data.Annotations;

import static java.lang.annotation.ElementType.METHOD;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target(METHOD)
@Retention(RetentionPolicy.RUNTIME)
public @interface BusinessMethod {
	String BusinessName() default "";
	String Keywords();
	String SubKeywords() default "";
	String TargetProject() default "";
	String Description() default "";
	boolean Default() default false;
}
